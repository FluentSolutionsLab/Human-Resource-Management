using CSharpFunctionalExtensions;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public sealed class Employee : Entity<Guid>
{
    public Name Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public DateOnly HireDate { get; }
    public DateOnly? TerminationDate { get; private set; }

    private Employee(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Id = Guid.NewGuid();
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

    public void Update(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));
        if (dateOfBirth == null) throw new ArgumentNullException(nameof(dateOfBirth));

        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
    }

    public void Terminate()
    {
        TerminationDate = DateOnly.FromDateTime(DateTime.Now);
    }
}