using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Persistence;

public class DatabaseInitializer
{
    public static void Initialize(PersonnelDbContext context)
    {
        if (context.Employees.Any()) return;
        var countries = new List<Employee>
        {
            Employee.Create(Name.Create("John", "Doe").Value, EmailAddress.Create("john.doe@gmail.com").Value, DateOfBirth.Create("1972-01-01").Value).Value,
            Employee.Create(Name.Create("Jane", "Doe").Value, EmailAddress.Create("jane.doe@gmail.com").Value, DateOfBirth.Create("1978-09-21").Value).Value,
            Employee.Create(Name.Create("Barthelemy", "Simpson").Value, EmailAddress.Create("barth.simpson@gmail.com").Value, DateOfBirth.Create("1982-03-11").Value).Value,
            Employee.Create(Name.Create("Donald", "Picsou").Value, EmailAddress.Create("donald.picsou@gmail.com").Value, DateOfBirth.Create("1962-11-01").Value).Value
        };
        context.AddRange(countries);
        context.SaveChanges();
    }
}