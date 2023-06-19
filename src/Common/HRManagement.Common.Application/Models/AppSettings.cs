namespace HRManagement.Common.Application.Models;

public class AppSettings
{
    public Database Database { get; set; } = null!;
    public bool IsDevEnvironment { get; set; }
}

public class Database
{
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string PersonnelManagement { get; set; } = null!;
}