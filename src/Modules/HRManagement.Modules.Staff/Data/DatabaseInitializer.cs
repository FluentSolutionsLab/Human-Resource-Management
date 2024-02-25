using Bogus;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Models;
using HRManagement.Modules.Staff.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Modules.Staff.Persistence;

public static class DatabaseInitializer
{
    public static Dictionary<string, Role> SeedDataRoles
    {
        get
        {
            var ceo = Role.Create(RoleName.Create("CEO").Value, null).Value;
            var president = Role.Create(RoleName.Create("President").Value, ceo).Value;
            var vicePresident = Role.Create(RoleName.Create("Vice President").Value, president).Value;
            var cto = Role.Create(RoleName.Create("CTO").Value, vicePresident).Value;
            var itManager = Role.Create(RoleName.Create("IT Manager").Value, cto).Value;
            var seManager = Role.Create(RoleName.Create("Software Engineering Manager").Value, itManager).Value;
            var architect = Role.Create(RoleName.Create("Architect").Value, itManager).Value;
            var leadDev = Role.Create(RoleName.Create("Lead Software Developer").Value, seManager).Value;
            var businessAnalyst = Role.Create(RoleName.Create("Business Analyst").Value, seManager).Value;
            var qaAnalyst = Role.Create(RoleName.Create("Quality Assurance Analyst").Value, seManager).Value;
            var seniorDev = Role.Create(RoleName.Create("Senior Software Developer").Value, leadDev).Value;
            var intermediateDev = Role.Create(RoleName.Create("Intermediate Software Developer").Value, leadDev).Value;
            var juniorDev = Role.Create(RoleName.Create("Junior Software Developer").Value, leadDev).Value;

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
                {"junior-dev", juniorDev}
            };
            return roles;
        }
    }

    public static void Initialize(StaffDbContext context)
    {
        var isInMemoryDb = context.Database.ProviderName.Contains("InMemory");

        if (!isInMemoryDb)
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        var roles = BuildRoles(isInMemoryDb);
        if ((isInMemoryDb && !context.Roles.Any()) || !isInMemoryDb)
        {
            context.AddRange(roles.Values.ToList());
            context.SaveChanges();
        }

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

        if ((isInMemoryDb && !context.Employees.Any()) || !isInMemoryDb)
        {
            context.AddRange(employees);
            context.SaveChangesAsync();
        }
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

    private static Dictionary<string, Role> BuildRoles(bool isInMemoryDb)
    {
        var roles = SeedDataRoles;

        if (isInMemoryDb)
        {
            byte index = 1;
            foreach (var role in roles.Values) role.SetId(index++);
        }

        return roles;
    }
}