using CSharpFunctionalExtensions;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class Employee : Entity<Guid>
{
    public Name Name { get; }
    public EmailAddress EmailAddress { get; }
    public DateOfBirth DateOfBirth { get; }
    public DateOnly HireDate { get; }
    public DateOnly? TerminationDate { get; }

    private Employee(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        HireDate = DateOnly.FromDateTime(DateTime.Now);
    }

    public static Result<Employee, Error> Create(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));
        if (dateOfBirth == null) throw new ArgumentNullException(nameof(dateOfBirth));

        return new Employee(name, emailAddress, dateOfBirth);
    }
}