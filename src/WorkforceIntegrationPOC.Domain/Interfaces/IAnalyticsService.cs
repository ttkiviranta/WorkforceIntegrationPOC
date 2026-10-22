using WorkforceIntegrationPOC.Domain.Entities;

namespace WorkforceIntegrationPOC.Domain.Interfaces;

/// <summary>
/// Provides the analysis abstraction used by the app.
/// A future Azure OpenAI client can be added here without changing the presentation layer.
/// </summary>
public interface IAnalyticsService
{
    Task<string> AnalyzeWorkforceAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetEmployeesForAnalysisAsync(CancellationToken cancellationToken = default);
}
