namespace HRManagement.Common.Domain.Utilities;

public static class Extensions
{
    public static string ToISO8601String(this DateOnly date)
    {
        return date.ToString("O");
    }
}