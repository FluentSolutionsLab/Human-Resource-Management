using System.ComponentModel.DataAnnotations;

namespace HRManagement.Common.Application.Models;

public class AppSettings
{
    [Required] public Database Database { get; set; }
}

public class Database
{
    [Required] public ConnectionStrings ConnectionStrings { get; set; }
    public bool ResetDbOnStart { get; set; }
}

public class ConnectionStrings
{
    [Required] public string PersonnelManagement { get; set; }
}