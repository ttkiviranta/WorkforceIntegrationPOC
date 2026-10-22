using WorkforceIntegrationPOC.Domain.Entities;

namespace WorkforceIntegrationPOC.Domain.Interfaces;

/// <summary>
/// Defines the abstraction for importing external workforce records into local storage.
/// This keeps the Web layer decoupled from the implementation details of the integration.
/// </summary>
public interface IIntegrationService
{
    Task<int> ImportSampleDataAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> FetchExternalEmployeesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Shift>> FetchExternalShiftsAsync(CancellationToken cancellationToken = default);
}
