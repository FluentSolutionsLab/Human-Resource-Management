namespace HRManagement.Common.Domain.Models;

public sealed class Error
{
    private const string Separator = "||";

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public string Serialize()
    {
        return $"{Code}{Separator}{Message}";
    }

    public static Error Deserialize(string serialized)
    {
        if (serialized == "A non-empty request body is required.")
            return new Error("value.is.required", "Value is required.");

        var data = serialized.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);

        if (data.Length < 2)
            throw new Exception($"Invalid error serialization: '{serialized}'");

        return new Error(data[0], data[1]);
    }
}