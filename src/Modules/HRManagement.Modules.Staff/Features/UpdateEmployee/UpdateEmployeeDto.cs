using System.Text.Json.Serialization;

namespace HRManagement.Modules.Staff.Features.UpdateEmployee;

public class UpdateEmployeeDto
{
    [JsonPropertyName("first_name")] public string FirstName { get; set; }
    [JsonPropertyName("last_name")] public string LastName { get; set; }
    [JsonPropertyName("email_address")] public string EmailAddress { get; set; }
    [JsonPropertyName("birthdate")] public string DateOfBirth { get; set; }
    [JsonPropertyName("hiring_date")] public string HiringDate { get; set; }
    [JsonPropertyName("role_id")] public byte RoleId { get; set; }
    [JsonPropertyName("manager_id")] public string ManagerId { get; set; }
}