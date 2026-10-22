namespace WorkforceIntegrationPOC.Domain.Entities;

/// <summary>
/// Represents a scheduled time block requiring a specific capability or service skill.
/// </summary>
public class Shift
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Location { get; set; } = string.Empty;

    public string RequiredSkill { get; set; } = string.Empty;

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
