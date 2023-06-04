using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using HRManagement.Modules.Personnel.Domain;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Modules.Personnel.Persistence;

public class DatabaseInitializer
{
    public static async Task InitializeAsync(PersonnelDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        
        var roles = BuildRoles();
        context.AddRange(roles.Values.ToList());
        await context.SaveChangesAsync();

        var ceo = CreateEmployee(roles["ceo"]);
        var president = CreateEmployee(roles["president"], ceo);
        var vicePresident1 = CreateEmployee(roles["vice-president"], president);
        var vicePresident2 = CreateEmployee(roles["vice-president"], president);
        var cto = CreateEmployee(roles["cto"], vicePresident1);
        var itManager = CreateEmployee(roles["it-manager"], cto);
        var seManager = CreateEmployee(roles["se-manager"], itManager);
        var architect = CreateEmployee(roles["architect"], itManager);

        var employees = new List<Employee>
        {
            ceo,
            president,
            vicePresident1,
            vicePresident2,
            cto,
            itManager,
            seManager,
            architect
        };

        var leads = new List<Employee>();
        for (var i = 0; i < 20; i++) 
            leads.Add(CreateEmployee(roles["lead-dev"], seManager));
        employees.AddRange(leads);
        
        leads.ForEach(lead =>
        {
            employees.Add(CreateEmployee(roles["business-analyst"], seManager));
            employees.Add(CreateEmployee(roles["business-analyst"], seManager));
            employees.Add(CreateEmployee(roles["qa-analyst"], seManager));
            employees.Add(CreateEmployee(roles["qa-analyst"], seManager));
            employees.Add(CreateEmployee(roles["senior-dev"], lead));
            employees.Add(CreateEmployee(roles["senior-dev"], lead));
            employees.Add(CreateEmployee(roles["intermediate-dev"], lead));
            employees.Add(CreateEmployee(roles["intermediate-dev"], lead));
            employees.Add(CreateEmployee(roles["junior-dev"], lead));
            employees.Add(CreateEmployee(roles["junior-dev"], lead));
        });

        context.AddRange(employees);
        await context.SaveChangesAsync();
    }

    private static Employee CreateEmployee(Role role, Employee manager = null)
    {
        var hiringDate = new Faker().Date.Past(15);
        var person = new Person();
        return Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value, 
            EmailAddress.Create(person.Email).Value, 
            ValueDate.Create(person.DateOfBirth.ToShortDateString()).Value, 
            ValueDate.Create(hiringDate.ToString("d")).Value,
            role,
            manager).Value;
    }

    private static Dictionary<string, Role> BuildRoles()
    {
        var ceo = Role.Create("CEO", null).Value;
        var president = Role.Create("President", ceo).Value;
        var vicePresident = Role.Create("Vice President", president).Value;
        var cto = Role.Create("CTO", vicePresident).Value;
        var itManager = Role.Create("IT Manager", cto).Value;
        var seManager = Role.Create("Software Engineering Manager", itManager).Value;
        var architect = Role.Create("Architect", itManager).Value;
        var leadDev = Role.Create("Lead Software Developer", seManager).Value;
        var businessAnalyst = Role.Create("Business Analyst", seManager).Value;
        var qaAnalyst = Role.Create("Quality Assurance Analyst", seManager).Value;
        var seniorDev = Role.Create("Senior Software Developer", leadDev).Value;
        var intermediateDev = Role.Create("Intermediate Software Developer", leadDev).Value;
        var juniorDev = Role.Create("Junior Software Developer", leadDev).Value;

        var roles = new Dictionary<string, Role>
        {
            {"ceo", ceo},
            {"president", president},
            {"vice-president", vicePresident},
            {"cto", cto},
            {"it-manager", itManager},
            {"se-manager", seManager},
            {"architect", architect},
            {"business-analyst", businessAnalyst},
            {"qa-analyst", qaAnalyst},
            {"lead-dev", leadDev},
            {"senior-dev", seniorDev},
            {"intermediate-dev", intermediateDev},
            {"junior-dev", juniorDev},
        };
        return roles;
    }
}