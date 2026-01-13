namespace App.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SkipWrappingAttribute : Attribute
{
}
