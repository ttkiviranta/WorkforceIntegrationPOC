using Microsoft.EntityFrameworkCore;
using WorkforceIntegrationPOC.Domain.Entities;
using WorkforceIntegrationPOC.Domain.Interfaces;
using WorkforceIntegrationPOC.Infrastructure.Data;

namespace WorkforceIntegrationPOC.Infrastructure.Services;

/// <summary>
/// Produces a mock AI staffing analysis using local database data.
/// This is intentionally structured so a future Azure OpenAI call can replace the text generation logic.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly WorkforceDbContext _dbContext;

    public AnalyticsService(WorkforceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> AnalyzeWorkforceAsync(CancellationToken cancellationToken = default)
    {
        var employees = await GetEmployeesForAnalysisAsync(cancellationToken);
        var shifts = await _dbContext.Shifts.AsNoTracking().ToListAsync(cancellationToken);
        var assignments = await _dbContext.Assignments.AsNoTracking().ToListAsync(cancellationToken);

        var totalCoverage = assignments.Count;
        var unfilledShifts = shifts.Count(s => !assignments.Any(a => a.ShiftId == s.Id));
        var skills = employees
            .SelectMany(e => e.Skills.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s)
            .ToList();

        var summary = $"Mock AI staffing analysis: {employees.Count} employees are available, {shifts.Count} shifts are tracked, and {totalCoverage} assignments are currently matched. " +
            $"{unfilledShifts} shift(s) remain unfilled. The available skill coverage includes: {string.Join(", ", skills)}. " +
            "Overall, staffing looks stable for the current schedule window, with the greatest opportunity to increase capacity around logistics and IT coverage.";

        // Future extension: replace this text generation with an Azure OpenAI call that analyzes the same dataset.
        return summary;
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesForAnalysisAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }
}
