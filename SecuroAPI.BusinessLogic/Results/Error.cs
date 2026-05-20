namespace SecuroAPI.BusinessLogic.Results;
/// <summary>
/// Creates an error type for the results pattern
/// </summary>
/// <param name="Code"></param>
/// <param name="Description"></param>
public readonly record struct Error(string Code, string Description)
{
    public static readonly Error None = new("", "");
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
}