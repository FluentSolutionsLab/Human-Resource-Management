namespace HRManagement.Common.Domain.Models;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public bool IsDevEnvironment { get; set; }
}

public class ConnectionStrings
{
    public string PersonnelManagement { get; set; } = null!;
}