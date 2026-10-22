namespace WorkforceIntegrationPOC.Domain.Entities;

/// <summary>
/// Represents a matching between an employee and a shift.
/// </summary>
public class Assignment
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int ShiftId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Shift Shift { get; set; } = null!;
}
