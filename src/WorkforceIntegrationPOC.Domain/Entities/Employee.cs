namespace WorkforceIntegrationPOC.Domain.Entities;

/// <summary>
/// Represents a workforce member with skills and availability information.
/// This is intentionally a generic model that can be mapped to different external schedules.
/// </summary>
public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Skills { get; set; } = string.Empty;

    public string Availability { get; set; } = string.Empty;

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
