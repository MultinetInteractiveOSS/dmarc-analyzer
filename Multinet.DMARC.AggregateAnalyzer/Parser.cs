using DnsClient;
using DnsClient.Protocol;
using Multinet.DMARC.AggregateAnalyzer.Schema;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Multinet.DMARC.AggregateAnalyzer
{
    public static class Parser
    {
        internal static LookupClient dnsClient = null;

        public static DMARCReport ParseXML(string xml, bool doDNSChecks = false)
        {
            var serializer = new XmlSerializer(typeof(DMARCReport));
            using (var reader = new StringReader(xml))
            {
                var report = serializer.Deserialize(reader) as DMARCReport;
                MakeSummary(ref report, doDNSChecks);
                return report;
            }
        }

        public static DMARCReport ParseFile(string path, bool doDNSChecks = false)
        {
            var serializer = new XmlSerializer(typeof(DMARCReport));
            using (var reader = new StreamReader(path))
            {
                var report = serializer.Deserialize(reader) as DMARCReport;
                MakeSummary(ref report, doDNSChecks);
                return report;
            }
        }

        private static void MakeSummary(ref DMARCReport report, bool doDNSChecks)
        {
            var summary = new ReportSummary
            {
                DMARC = new Dictionary<string, List<RecordType>>(),
                Forwarder = new Dictionary<string, List<RecordType>>(),
                Unknown = new Dictionary<string, List<RecordType>>()
            };

            Hashtable checkedHosts = new Hashtable();

            var groupedSources = report.Records.GroupBy(g => g.Row.SourceIp);

            foreach (var groupedSource in groupedSources)
            {
                var senderHost = groupedSource.Key;
                if (doDNSChecks)
                {
                    if (dnsClient == null)
                    {
                        dnsClient = new LookupClient();
                    }

                    if (!checkedHosts.ContainsKey(groupedSource.Key))
                    {
                        var senderDns = dnsClient.QueryReverse(System.Net.IPAddress.Parse(groupedSource.Key)).Answers;
                        checkedHosts.Add(groupedSource.Key, senderDns);
                    }

                    var senderHostEntries = checkedHosts[groupedSource.Key] as IReadOnlyList<DnsResourceRecord>;
                    senderHost = senderHostEntries.PtrRecords().First().PtrDomainName.Value;
                }

                var groupedRecords = groupedSource.GroupBy(g => new
                {
                    Disposition = g.Row.PolicyEvaluated.Disposition,
                    DKIMPass = g.Row.PolicyEvaluated.DKIM == DMARCResultType.Pass,
                    SPFPass = g.Row.PolicyEvaluated.SPF == DMARCResultType.Pass,
                    DMARCPass = g.Row.PolicyEvaluated.DKIM == DMARCResultType.Pass || g.Row.PolicyEvaluated.SPF == DMARCResultType.Pass
                });

                foreach (var groupRecord in groupedRecords)
                {
                    summary.TotalVolume += groupRecord.Sum(r => r.Row.Count);

                    bool potentialForwarder = false;
                    bool maybeCorrect = false;

                    if (groupRecord.Key.DKIMPass)
                    {
                        summary.DKIMVolume += groupRecord.Sum(r => r.Row.Count);
                    }

                    if (groupRecord.Key.SPFPass)
                    {
                        summary.SPFVolume += groupRecord.Sum(r => r.Row.Count);
                    }
                    else
                    {
                        foreach (var record in groupRecord)
                        {
                            foreach (var spf in record.AuthResults.SPF)
                            {
                                if (spf.Result == SPFResultType.Fail)
                                {
                                    if (groupRecord.Key.DKIMPass)
                                    {
                                        summary.ForwarderVolume += record.Row.Count;
                                        summary.TotalVolume -= record.Row.Count;
                                        summary.DKIMVolume -= record.Row.Count;

                                        potentialForwarder = true;

                                        if (!summary.Forwarder.ContainsKey(senderHost))
                                        {
                                            summary.Forwarder.Add(senderHost, new List<RecordType>());
                                        }
                                        summary.Forwarder[senderHost].Add(record);
                                    }
                                }
                                else if (spf.Result == SPFResultType.None)
                                {
                                    if (!summary.DMARC.ContainsKey(senderHost))
                                    {
                                        summary.DMARC.Add(senderHost, new List<RecordType>());
                                    }
                                    summary.DMARC[senderHost].Add(record);
                                    maybeCorrect = true;
                                }
                            }
                        }
                    }

                    if (groupRecord.Key.DMARCPass && !potentialForwarder)
                    {
                        summary.DMARCVolume += groupRecord.Sum(r => r.Row.Count);
                        foreach (var record in groupRecord)
                        {
                            if (!summary.DMARC.ContainsKey(senderHost))
                            {
                                summary.DMARC.Add(senderHost, new List<RecordType>());
                            }
                            summary.DMARC[senderHost].Add(record);
                        }
                    }

                    if (!groupRecord.Key.DMARCPass
                    && !groupRecord.Key.SPFPass
                    && !groupRecord.Key.DKIMPass
                    && !maybeCorrect)
                    {
                        summary.UnknownVolume += groupRecord.Sum(r => r.Row.Count);
                        foreach (var record in groupRecord)
                        {
                            if (!summary.Unknown.ContainsKey(senderHost))
                            {
                                summary.Unknown.Add(senderHost, new List<RecordType>());
                            }
                            summary.Unknown[senderHost].Add(record);
                        }
                    }
                }
            }

            report.ReportSummary = summary;
        }
    }
}
