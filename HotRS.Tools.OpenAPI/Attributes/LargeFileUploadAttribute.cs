namespace HotRS.Tools.OpenAPI.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class LargeFileUploadAttribute : Attribute
{
    public string ParameterName { get; }

    public bool Required { get; }

    public LargeFileUploadAttribute(string parameterName = "file", bool required = true)
    {
        ParameterName = parameterName;
        Required = required;
    }
}