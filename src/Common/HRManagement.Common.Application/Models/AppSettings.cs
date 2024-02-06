using System.ComponentModel.DataAnnotations;

namespace HRManagement.Common.Application.Models;

public class AppSettings
{
    [Required] public ConnectionStrings ConnectionStrings { get; set; }
}

public class Database
{
    [Required] public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    [Required] public string PersonnelManagement { get; set; }
}