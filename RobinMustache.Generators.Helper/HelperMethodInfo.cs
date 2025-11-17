namespace RobinMustache.Generators.Accessor
{
    internal record struct HelperMethodInfo
    {
        public string HelperName { get; internal set; }
        public HelperAccessibility HelperAccessibility { get; internal set; }
        public string OutputTypeName { get; internal set; }
        public HelperArgumentInfo[] Arguments { get; internal set; }
    }
}