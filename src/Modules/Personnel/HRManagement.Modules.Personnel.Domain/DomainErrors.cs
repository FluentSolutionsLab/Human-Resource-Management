namespace HRManagement.Modules.Personnel.Domain;

public static class DomainErrors
{
    public static Error DateOfBirthInFuture()
    {
        return new Error("value.not.valid", "Your date of birth cannot be in the future");
    }

    public static Error InvalidEmailAddress(string email)
    {
        return new Error("value.not.valid", $"{email} is not a valid email address");
    }

    public static Error InvalidName(string name)
    {
        return new Error("value.not.valid", $"{name} is not a valid name");
    }

    public static Error NullOrEmptyName(string fieldName)
    {
        return new Error("value.not.valid", $"The field {fieldName} cannot be null or empty.");
    }
}