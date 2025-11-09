namespace Robin.Generators.Accessor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class GenerateAccessorAttribute : Attribute
    {
        public bool UseDelegates { get; set; }
    }
}