﻿using System.Collections.Generic;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class UpdateEmployeeCommand : ICommand<UnitResult<List<Error>>>
{
    public string EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string HiringDate { get; set; }
    public byte RoleId { get; set; }
    public string ReportsToId { get; set; }
}