using HRManagement.Modules.Personnel.Domain.Employee;
using HRManagement.Modules.Personnel.Domain.Role;

namespace HRManagement.Modules.Personnel.Persistence;

public class DatabaseInitializer
{
    public static void Initialize(PersonnelDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        var roleCeo = Role.Create("CEO", null).Value;
        var rolePresident = Role.Create("President", roleCeo).Value;
        var roleVicePresident = Role.Create("Vice President", rolePresident).Value;

        context.Add(roleCeo);
        context.Add(rolePresident);
        context.Add(roleVicePresident);
        context.SaveChanges();

        var ceo = Employee.Create(
            Name.Create("John", "Doe").Value, 
            EmailAddress.Create("john.doe@gmail.com").Value, 
            DateOfBirth.Create("1972-01-01").Value, 
            roleCeo, null).Value;
        var president = Employee.Create(
            Name.Create("Jane", "Doe").Value, 
            EmailAddress.Create("jane.doe@gmail.com").Value, 
            DateOfBirth.Create("1978-09-21").Value, 
            rolePresident, ceo).Value;
        var vicePresident1 = Employee.Create(
            Name.Create("Barthelemy", "Simpson").Value, 
            EmailAddress.Create("barth.simpson@gmail.com").Value, 
            DateOfBirth.Create("1982-03-11").Value,
            roleVicePresident, president).Value;
        var vicePresident2 = Employee.Create(
            Name.Create("Donald", "Picsou").Value, 
            EmailAddress.Create("donald.picsou@gmail.com").Value, 
            DateOfBirth.Create("1962-11-01").Value, 
            roleVicePresident, president).Value;
        var employees = new List<Employee>
        {
            ceo,
            president,
            vicePresident1,
            vicePresident2
        };

        context.AddRange(employees);
        context.SaveChanges();
    }
}