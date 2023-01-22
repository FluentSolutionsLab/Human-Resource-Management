using HRManagement.Modules.Personnel.Domain.Employee;
using HRManagement.Modules.Personnel.Domain.Role;

namespace HRManagement.Modules.Personnel.Persistence;

public class DatabaseInitializer
{
    public static void Initialize(PersonnelDbContext context)
    {
        if (context.Employees.Any() && context.Roles.Any()) return;
        
        var ceo = Role.Create("CEO", null).Value;
        var president = Role.Create("President", ceo).Value;
        var vicePresident = Role.Create("Vice President", president).Value;
        
        var countries = new List<Employee>
        {
            Employee.Create(
                Name.Create("John", "Doe").Value, 
                EmailAddress.Create("john.doe@gmail.com").Value, 
                DateOfBirth.Create("1972-01-01").Value, 
                ceo).Value,
            Employee.Create(
                Name.Create("Jane", "Doe").Value, 
                EmailAddress.Create("jane.doe@gmail.com").Value, 
                DateOfBirth.Create("1978-09-21").Value, 
                president).Value,
            Employee.Create(
                Name.Create("Barthelemy", "Simpson").Value, 
                EmailAddress.Create("barth.simpson@gmail.com").Value, 
                DateOfBirth.Create("1982-03-11").Value,
                vicePresident).Value,
            Employee.Create(
                Name.Create("Donald", "Picsou").Value, 
                EmailAddress.Create("donald.picsou@gmail.com").Value, 
                DateOfBirth.Create("1962-11-01").Value, 
                vicePresident).Value
        };
        context.Add(president);
        context.Add(vicePresident);
        context.AddRange(countries);
        context.SaveChanges();
    }
}