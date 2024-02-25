using System.Text.Json.Serialization;

namespace HRManagement.Modules.Staff.Features.FindEmployees;

public class EmployeeManagerDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("first_name")] public string FirstName { get; set; }
    [JsonPropertyName("last_name")] public string LastName { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; }
}