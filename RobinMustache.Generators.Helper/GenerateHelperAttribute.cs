namespace RobinMustache.Generators.Accessor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class GenerateHelperAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HelpersAttribute : Attribute;
}