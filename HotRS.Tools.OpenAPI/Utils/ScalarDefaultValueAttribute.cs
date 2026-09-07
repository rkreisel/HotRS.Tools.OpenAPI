namespace HotRS.Tools.OpenAPI.Utils;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ScalarDefaultValueAttribute(object value) : Attribute
{
    public object Value { get; } = value;
}
