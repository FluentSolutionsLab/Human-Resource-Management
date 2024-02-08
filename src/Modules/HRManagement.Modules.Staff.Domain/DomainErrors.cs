using HRManagement.Common.Domain.Models;

namespace HRManagement.Modules.Staff.Domain;

public static class DomainErrors
{
    public static Error DateInFuture()
    {
        return new Error("value.not.valid", "The date cannot be in the future");
    }

    public static Error InvalidEmailAddress(string email)
    {
        return new Error("value.not.valid", $"{email} is not a valid email address");
    }

    public static Error InvalidInput(string input)
    {
        return new Error("value.not.valid", $"{input} is not a valid value");
    }

    public static Error NullOrEmptyName(string fieldName)
    {
        return new Error("value.not.valid", $"The field {fieldName} cannot be null or empty.");
    }

    public static Error NotFound(string entityName, object id)
    {
        return new Error("resource.not.found", $"No match found for the resource '{entityName}' with Id '{id}'.");
    }

    public static Error ResourceAlreadyExists()
    {
        return new Error("resource.already.exists", "A resource with the same key attributes already exists.");
    }

    public static Error InvalidDate(string date)
    {
        return new Error("value.not.valid", $"{date} is not a valid date");
    }

    public static Error EmployeeCannotReportToSameRole()
    {
        return new Error("invalid.operation", "An employee cannot report to another one with the same role.");
    }

    public static Error ManagerRoleMustComplyWithOrganization()
    {
        return new Error("invalid.operation", "The manager role does not comply with the organization chart.");
    }
}