using System;
using System.Collections.Generic;

namespace ClassLibraryNetStandard2
{
    public abstract class BaseDemoClass
    {
        public abstract IEnumerable<(string Name, Func<string, object> Resolver)> KeyParametersWithResolver { get; }
    }
}
