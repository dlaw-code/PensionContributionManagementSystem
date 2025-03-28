namespace PensionContributionManagementSystem.Core.Dtos;

public class Error
{
    public static readonly IEnumerable<Error> None = Array.Empty<Error>();
    public static readonly Error NullValue = new("Error.NullValue", "the specified result value is null.");

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Message { get; }
    public string Code { get; }
}