using System;
using System.Collections.Generic;
using ClassLibraryNetStandard2;

namespace ValueTupleIssueDemo
{
    internal class DemoClass : BaseDemoClass
    {
        public override IEnumerable<(string Name, Func<string, object> Resolver)> KeyParametersWithResolver { get; }

        public DemoClass()
        {
            KeyParametersWithResolver = new (string Name, Func<string, object> Resolver)[]
           {
                ("key", key => key)
           };
        }
    }
}
