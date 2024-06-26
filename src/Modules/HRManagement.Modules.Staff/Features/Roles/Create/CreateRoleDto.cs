﻿using System.Text.Json.Serialization;

namespace HRManagement.Modules.Staff.Features.Roles.Create;

public class CreateRoleDto
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("manager_role_id")] public byte? ReportsToId { get; set; }
}