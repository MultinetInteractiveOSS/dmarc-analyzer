using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Multinet.DMARC.AggregateAnalyzer.Schema;
using Multinet.DMARC.Backend.Database;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System.Buffers;
using System.IO.Compression;
using System.Text.Json;

internal class DMARCParserStore : MessageStore
{
    private readonly ILogger<DMARCParserStore> _logger;
    private readonly ReportContext _context;

    public DMARCParserStore(ReportContext context, ILogger<DMARCParserStore> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"New incoming message: {transaction.From} -> {transaction.To}");
        await using var stream = new MemoryStream();

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory))
        {
            await stream.WriteAsync(memory, cancellationToken);
        }

        stream.Position = 0;

        try
        {
            using var mail = await MimeMessage.LoadAsync(stream, cancellationToken);
            _logger.LogDebug(_logger.IsEnabled(LogLevel.Debug) ? mail.ToString() : mail.MessageId);
            bool foundAttachment = false;
            foreach (var attachment in mail.Attachments)
            {
                if (attachment.IsAttachment)
                {
                    switch (attachment.ContentType.MimeType)
                    {
                        case "application/gzip":
                            await handleGzipReport(mail, (MimePart)attachment);
                            break;
                        case "application/zip":
                            await handleZipReport(mail, (MimePart)attachment);
                            break;
                    }
                }
            }

            if (!foundAttachment)
            {
                foreach (var bodyPart in mail.BodyParts)
                {
                    switch (bodyPart.ContentType.MimeType)
                    {
                        case "application/gzip":
                            await handleGzipReport(mail, (MimePart)bodyPart);
                            break;
                        case "application/zip":
                            await handleZipReport(mail, (MimePart)bodyPart);
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log exceptions
            _logger.LogError(ex, "Error while processing message");
        }

        return SmtpResponse.Ok;
    }

    async Task handleZipReport(MimeMessage mail, MimePart attachment)
    {
        _logger.LogDebug("Found ZIP attachment");
        using var ms = attachment.Content.Open();
        using var bs = new BufferedStream(ms);
        using var zip = new ZipArchive(bs, ZipArchiveMode.Read, true);
        foreach (var entry in zip.Entries)
        {
            using var fr = new StreamReader(entry.Open());
            var xml = await fr.ReadToEndAsync();
            var report = Multinet.DMARC.AggregateAnalyzer.Parser.ParseXML(xml);
            await storeReport(report, xml);
        }
    }

    async Task handleGzipReport(MimeMessage mail, MimePart attachment)
    {
        _logger.LogDebug("Found GZIP attachment");
        using var ms = attachment.Content.Open();
        using var bs = new BufferedStream(ms);
        using var zip = new GZipStream(bs, CompressionMode.Decompress, true);
        using var sr = new StreamReader(zip);
        var xml = await sr.ReadToEndAsync();
        var report = Multinet.DMARC.AggregateAnalyzer.Parser.ParseXML(xml);
        await storeReport(report, xml);
    }

    async Task storeReport(DMARCReport report, string reportXml)
    {
        var reportJson = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
        });

        if (report.ReportMetadata.DateRange == null)
        {
            _logger.LogWarning($"Report {report.ReportMetadata.ReportId} has no date range, skipping");
            return;
        }

        if (await _context.Reports.AnyAsync(r => r.ReportId == report.ReportMetadata.ReportId && r.Email == report.ReportMetadata.Email))
        {
            _logger.LogDebug($"Report {report.ReportMetadata.ReportId} already exists, skipping");
            return;
        }

        var _report = new Report
        {
            Id = Guid.NewGuid(),
            ReportId = report.ReportMetadata.ReportId,
            OrganizationName = report.ReportMetadata.OrganizationName,
            Email = report.ReportMetadata.Email,
            ExtraContactInfo = report.ReportMetadata.ExtraContactInfo,
            DateRangeBegin = report.ReportMetadata.DateRange.Begin,
            DateRangeEnd = report.ReportMetadata.DateRange.End,
            Domain = report.PolicyPublished.Domain,
            ReportIngested = DateTimeOffset.UtcNow,
            TotalVolume = report.ReportSummary.TotalVolume,
            DKIMVolume = report.ReportSummary.DKIMVolume,
            SPFVolume = report.ReportSummary.SPFVolume,
            DMARCVolume = report.ReportSummary.DMARCVolume,
            ForwarderVolume = report.ReportSummary.ForwarderVolume,
            UnknownVolume = report.ReportSummary.UnknownVolume,
            ReportJson = reportJson,
            ReportRawData = reportXml
        };

        _context.Reports.Add(_report);

        await _context.SaveChangesAsync();

        _logger.LogDebug($"Report {report.ReportMetadata.ReportId} saved");
    }
}
