using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ClrMdExample
{
    partial class Program
    {
        static string CreateSampleApplication() =>
                new CSharpCodeProvider().CompileAssemblyFromSource
                                         (
                                             new CompilerParameters
                                             {
                                                 GenerateExecutable = true,
                                                 OutputAssembly = "Generated.exe",
                                                 CompilerOptions = "/platform:x86"
                                             },
@"
using System;

class Program
{
    public static void Main()
    {
        Console.WriteLine(""Ciao2"");

        Console.ReadKey();                        
    }
}
"                                        )
                                        .PathToAssembly;
    }
}