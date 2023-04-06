## Steps to reproduce the issue

1. Have System.ValueTuple netstandard1.0 dll in GAC
(can be taken from https://www.nuget.org/packages/System.ValueTuple/)
2. Create console app targeting net48
3. Have auto generated binding redirects ON for the console app project.
4. Add nuget package *Microsoft.Extensions.Configuration.Json* v. 3.1.22  (version is not relevant, can be newer as well)
5. Have class with property using System.ValueTuple like this:

```
internal class DemoClass
    {
        public IEnumerable<(string Name, Func<string, object> Resolver)> KeyParametersWithResolver { get; }

        public DemoClass()
        {
            KeyParametersWithResolver = new (string Name, Func<string, object> Resolver)[]
           {
                ("key", key => key)
           };
        }
    }
```

6. Instantiate this class from Main in Program.cs

```
 DemoClass demoClass = new DemoClass();
```

Until now, program should be running just fine.

7. Add ClassLibrary project targeting netstandard2.0 and reference it from console app project.
8. Add abstract class including abstract property

```
 public abstract class BaseDemoClass
    {
        public abstract IEnumerable<(string Name, Func<string, object> Resolver)> KeyParametersWithResolver { get; }
    }
```

9. As soon as you implement this abstract class in DemoClass like this:

```
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
```

You'll get:
*Unhandled Exception: System.TypeLoadException: Method 'get_KeyParametersWithResolver' in type 'ValueTupleIssueDemo.DemoClass' from assembly 'ValueTupleIssueDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' does not have an implementation.
   at ValueTupleIssueDemo.Program.Main(String[] args)*


## Description
System.ValueTuple is from NET 4.7 part of mscorlib, up to that point package was needed. Here partial issue is that *Microsoft.Extensions.Configuration.Json* has dependency on *System.Text.Json*
which then refers to *System.ValueTuple*, assembly redirection is then generated automatically

```
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.ValueTuple" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.0.3.0" newVersion="4.0.3.0" />
      </dependentAssembly>
    </assemblyBinding>
```
net47 and netstandard1.0 dll for System.ValueTuple have same signature that is token, culture, version etc. Only difference is that net47 version has forwarder
to mscorlib, while netstandard1.0 one has implementation of ValueTuple in it. 

Because of the same signature and order of assembly binding, the dll from GAC is loaded.
**IMPORTANT** : but that is **ONLY IN CASE** where we implement class from net standard 2.0, without it the mscorlib is used directly.

NOTE: Because the signature is same and only internal implementation differs, FusionLog will not show any issue.

## Possible workarounds right now
1. Replacing System.ValueTuple.dll netstandard1.0 with System.ValueTuple.dll net47 in GAC
2. Deletion of binding redirect from *ValueTupleIssueDemo.exe.config* also is enough (after autogeneration)
3. Replace value.tuple with tuple
