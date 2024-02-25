using System.Text.Json.Serialization;

namespace HRManagement.Modules.Staff.Features.FindEmployees;

public class EmployeeDto
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("first_name")] public string FirstName { get; set; }
    [JsonPropertyName("last_name")] public string LastName { get; set; }
    [JsonPropertyName("email_address")] public string EmailAddress { get; set; }
    [JsonPropertyName("birthdate")] public string DateOfBirth { get; set; }
    [JsonPropertyName("hire_date")] public string HireDate { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; }
    [JsonPropertyName("manager")] public EmployeeManagerDto Manager { get; set; }
}