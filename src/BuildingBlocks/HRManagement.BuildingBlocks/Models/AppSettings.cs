using System.ComponentModel.DataAnnotations;

namespace HRManagement.BuildingBlocks.Models;

public class AppSettings
{
    [Required] public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    [Required] public string Default { get; set; }
}