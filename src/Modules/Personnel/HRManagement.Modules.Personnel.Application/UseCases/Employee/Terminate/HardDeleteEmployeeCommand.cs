﻿using CSharpFunctionalExtensions;
using HRManagement.Common.Application.Contracts;
using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Personnel.Application.UseCases;

public class HardDeleteEmployeeCommand : ICommand<UnitResult<Error>>
{
    public string EmployeeId { get; set; }
}