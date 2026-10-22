using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WorkforceIntegrationPOC.Domain.Interfaces;
using WorkforceIntegrationPOC.Infrastructure.Data;
using WorkforceIntegrationPOC.Infrastructure.Services;
using WorkforceIntegrationPOC.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Register the application services in a layered, dependency-injected configuration.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<WorkforceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=workforce.db"));

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
    dbContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Minimal API endpoints for the demonstration workflow.
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", message = "Workforce integration demo is running." }));

app.MapGet("/api/employees", async (WorkforceDbContext dbContext) =>
    Results.Ok(await dbContext.Employees.AsNoTracking().OrderBy(e => e.Name).ToListAsync()));

app.MapGet("/api/shifts", async (WorkforceDbContext dbContext) =>
    Results.Ok(await dbContext.Shifts.AsNoTracking().OrderBy(s => s.StartTime).ToListAsync()));

app.MapGet("/api/assignments", async (WorkforceDbContext dbContext) =>
    Results.Ok(await dbContext.Assignments
        .AsNoTracking()
        .Include(a => a.Employee)
        .Include(a => a.Shift)
        .OrderBy(a => a.Shift.StartTime)
        .ToListAsync()));

app.MapPost("/api/import", async (IIntegrationService integrationService) =>
{
    var importedCount = await integrationService.ImportSampleDataAsync();
    return Results.Ok(new { imported = importedCount, message = "Sample workforce data imported." });
});

app.MapPost("/api/analyze", async (IAnalyticsService analyticsService) =>
{
    var summary = await analyticsService.AnalyzeWorkforceAsync();
    return Results.Ok(new { summary });
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
