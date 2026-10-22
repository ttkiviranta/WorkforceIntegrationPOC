using Microsoft.EntityFrameworkCore;
using WorkforceIntegrationPOC.Domain.Entities;
using WorkforceIntegrationPOC.Domain.Interfaces;
using WorkforceIntegrationPOC.Infrastructure.Data;

namespace WorkforceIntegrationPOC.Infrastructure.Services;

/// <summary>
/// Simulates an external workforce integration.
/// In a real implementation this would call a third-party API or ERP endpoint and map the payload.
/// </summary>
public class IntegrationService : IIntegrationService
{
    private readonly WorkforceDbContext _dbContext;

    public IntegrationService(WorkforceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> ImportSampleDataAsync(CancellationToken cancellationToken = default)
    {
        var employees = await FetchExternalEmployeesAsync(cancellationToken);
        var shifts = await FetchExternalShiftsAsync(cancellationToken);

        if (!employees.Any() || !shifts.Any())
        {
            return 0;
        }

        var employeeLookup = await _dbContext.Employees
            .ToDictionaryAsync(e => e.Name, cancellationToken);

        foreach (var employee in employees)
        {
            if (!employeeLookup.ContainsKey(employee.Name))
            {
                _dbContext.Employees.Add(employee);
            }
        }

        var existingShifts = await _dbContext.Shifts
            .ToListAsync(cancellationToken);

        foreach (var shift in shifts)
        {
            if (!existingShifts.Any(s => s.Location == shift.Location && s.StartTime == shift.StartTime && s.EndTime == shift.EndTime))
            {
                _dbContext.Shifts.Add(shift);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var persistedEmployees = await _dbContext.Employees
            .Include(e => e.Assignments)
            .ToListAsync(cancellationToken);

        var persistedShifts = await _dbContext.Shifts
            .ToListAsync(cancellationToken);

        _dbContext.Assignments.RemoveRange(_dbContext.Assignments);

        var assignments = CreateAssignments(persistedEmployees, persistedShifts);
        _dbContext.Assignments.AddRange(assignments);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return assignments.Count;
    }

    public Task<IReadOnlyList<Employee>> FetchExternalEmployeesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var employees = new List<Employee>
        {
            new() { Name = "Alicia Gomez", Skills = "Nurse, First Aid, Patient Care", Availability = "Weekdays 08:00-16:00" },
            new() { Name = "Marcus Chen", Skills = "Logistics, Forklift, Inventory", Availability = "Weekdays 10:00-18:00" },
            new() { Name = "Priya Patel", Skills = "IT Support, Security, Troubleshooting", Availability = "Flexible" },
            new() { Name = "Daniel Brooks", Skills = "Field Service, Safety, Reporting", Availability = "Mon-Wed-Fri" }
        };

        return Task.FromResult<IReadOnlyList<Employee>>(employees);
    }

    public Task<IReadOnlyList<Shift>> FetchExternalShiftsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var today = DateTime.Today;
        var shifts = new List<Shift>
        {
            new() { StartTime = today.AddHours(8), EndTime = today.AddHours(16), Location = "North Wing", RequiredSkill = "Nurse" },
            new() { StartTime = today.AddHours(9), EndTime = today.AddHours(17), Location = "Warehouse", RequiredSkill = "Logistics" },
            new() { StartTime = today.AddHours(10), EndTime = today.AddHours(18), Location = "IT Desk", RequiredSkill = "IT Support" },
            new() { StartTime = today.AddHours(11), EndTime = today.AddHours(15), Location = "Field Ops", RequiredSkill = "Field Service" }
        };

        return Task.FromResult<IReadOnlyList<Shift>>(shifts);
    }

    private static List<Assignment> CreateAssignments(List<Employee> employees, List<Shift> shifts)
    {
        var assignments = new List<Assignment>();
        var employeeSkillMap = employees
            .Select(e => new { e.Id, Skills = e.Skills.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) })
            .ToList();

        for (var i = 0; i < shifts.Count; i++)
        {
            var requiredSkill = shifts[i].RequiredSkill;
            var match = employeeSkillMap
                .FirstOrDefault(e => e.Skills.Any(skill => skill.Equals(requiredSkill, StringComparison.OrdinalIgnoreCase)));

            if (match is null)
            {
                continue;
            }

            assignments.Add(new Assignment
            {
                EmployeeId = match.Id,
                ShiftId = shifts[i].Id,
                Employee = employees.First(e => e.Id == match.Id),
                Shift = shifts[i]
            });
        }

        return assignments;
    }
}
