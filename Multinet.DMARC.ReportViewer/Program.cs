using Microsoft.EntityFrameworkCore;
using Multinet.DMARC.Backend.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<ReportContext>(options =>
{
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
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
