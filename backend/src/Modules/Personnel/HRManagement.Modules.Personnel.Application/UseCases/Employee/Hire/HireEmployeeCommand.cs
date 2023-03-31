﻿using System.Collections.Generic;
using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HireEmployeeCommand : ICommand<Result<EmployeeDto, List<Error>>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public byte RoleId { get; set; }
    public string ReportsToId { get; set; }
}