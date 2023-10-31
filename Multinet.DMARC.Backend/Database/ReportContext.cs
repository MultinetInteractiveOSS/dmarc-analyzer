using Microsoft.EntityFrameworkCore;
using Multinet.DMARC.AggregateAnalyzer.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multinet.DMARC.Backend.Database
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options) : base(options)
        {
        }

        public virtual DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("DMARC_Reports");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => new
                {
                    e.ReportId,
                    e.Email,
                    e.DateRangeBegin,
                    e.DateRangeEnd,
                    e.Domain
                });

                entity.HasIndex(e => e.Domain);
                entity.HasIndex(e => new
                {
                    e.DateRangeBegin,
                    e.DateRangeEnd
                });

                entity.Property(e => e.ReportId)
                    .HasColumnName("ReportId")
                    .HasMaxLength(512)
                    .IsRequired();

                entity.Property(e => e.OrganizationName)
                .HasColumnName("OrganizationName")
                    .HasMaxLength(512)
                    .IsRequired();

                entity.Property(e => e.Email)
                .HasColumnName("Email")
                    .HasMaxLength(512)
                    .IsRequired();

                entity.Property(e => e.ExtraContactInfo)
                .HasColumnName("ExtraContactInfo")
                    .HasMaxLength(512);

                entity.Property(e => e.DateRangeBegin)
                .HasColumnName("DateRangeBegin")
                    .IsRequired();

                entity.Property(e => e.DateRangeEnd)
                .HasColumnName("DateRangeEnd")
                    .IsRequired();

                entity.Property(e => e.Domain)
                .HasColumnName("Domain")
                    .HasMaxLength(512)
                    .IsRequired();
            });
        }
    }

    public class Report
    {
        public required Guid Id { get; set; }
        public required string ReportId { get; set; }
        public required DateTimeOffset ReportIngested { get; set; } = DateTimeOffset.UtcNow;
        public required string OrganizationName { get; set; }
        public required string Email { get; set; }
        public string? ExtraContactInfo { get; set; }
        public required long DateRangeBegin { get; set; }
        [NotMapped]
        public DateTimeOffset DateRangeBeginDateTime => DateTimeOffset.FromUnixTimeSeconds(DateRangeBegin);
        public required long DateRangeEnd { get; set; }
        [NotMapped]
        public DateTimeOffset DateRangeEndDateTime => DateTimeOffset.FromUnixTimeSeconds(DateRangeEnd);
        public required string Domain { get; set; }
        public required long TotalVolume { get; set; }
        public required long DMARCVolume { get; set; }
        public required long SPFVolume { get; set; }
        public required long DKIMVolume { get; set; }
        public required long ForwarderVolume { get; set; }
        public required long UnknownVolume { get; set; }
        public required string ReportJson { get; set; }
        [NotMapped]
        public DMARCReport DMARCReport => System.Text.Json.JsonSerializer.Deserialize<DMARCReport>(ReportJson)!;
    }

    public enum DatabaseType
    {
        Sqlite,
        Postgres,
        MySql,
        SqlServer,
        InMemory
    }
}
