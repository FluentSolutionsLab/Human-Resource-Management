﻿using System.Text.Json.Serialization;

namespace HRManagement.Modules.Staff.Features.FindRoles;

public class RoleDto
{
    [JsonPropertyName("id")] public byte Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("manager_role")] public string ReportsTo { get; set; }
}