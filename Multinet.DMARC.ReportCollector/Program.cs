using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Multinet.DMARC.Backend.Database;
using SmtpServer;

var builder = Host.CreateApplicationBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
}).CreateLogger("Program");

logger.LogInformation("Launching Multinet.DMARC.ReportCollector");

logger.LogInformation("Loading configuration");

logger.LogInformation("Building service provider");

builder.Services.AddDbContextPool<ReportContext>(options =>
{
    logger.LogInformation("Configuring database");
    var dbSection = builder.Configuration.GetSection("Database");
    if (!Enum.TryParse(typeof(DatabaseType), dbSection["Type"], out var _databaseType))
    {
        throw new Exception("Invalid database type");
    }

    var _connectionString = dbSection["ConnectionString"]!;

    switch (_databaseType)
    {
        case DatabaseType.Sqlite:
            options.UseSqlite(_connectionString);
            break;
        case DatabaseType.Postgres:
            options.UseNpgsql(_connectionString);
            break;
        case DatabaseType.MySql:
            options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            break;
        case DatabaseType.SqlServer:
            options.UseSqlServer(_connectionString);
            break;
        case DatabaseType.InMemory:
            options.UseInMemoryDatabase(_connectionString);
            break;
    }

    options
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();

    logger.LogInformation($"Database configured, {_databaseType}");
});

using var host = builder.Build();

logger.LogInformation("Making sure database is created and migrated");

var context = host.Services.GetRequiredService<ReportContext>();

if (environmentName == "Development")
{
    //await context.Database.EnsureDeletedAsync();
}

await context.Database.EnsureCreatedAsync();
await context.Database.MigrateAsync();

logger.LogInformation("Database ready for action");

logger.LogInformation("Configuring SMTP server");

var options = new SmtpServerOptionsBuilder()
    .ServerName("localhost")
    .Port(25, 587)
    .Build();

var smtpServiceProvider = new SmtpServer.ComponentModel.ServiceProvider();
smtpServiceProvider.Add(new DMARCParserStore(host.Services.GetRequiredService<ReportContext>(), host.Services.GetRequiredService<ILogger<DMARCParserStore>>()));

var smtpServer = new SmtpServer.SmtpServer(options, smtpServiceProvider);

logger.LogInformation("Starting DMARC report collector");
await smtpServer.StartAsync(CancellationToken.None);

await host.RunAsync();