using System;

namespace RobinMustache.Generators.Accessor
{

    internal record struct HelpersInfo
    {
        public string TypeNamespaceName { get; internal set; }
        public string TypeName { get; internal set; }
        public string Accessibility { get; internal set; }
    }
}