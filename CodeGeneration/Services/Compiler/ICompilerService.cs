using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeGeneration.Services.Compiler
{
    public interface ICompilerService
    {
        IDictionary<string, Type> GetCompiledTypeFromSource(IDictionary<string, string> sources);
        Type GetCompiledTypeFromSource(string modelName, string source);
        IDictionary<string, CompilerResults> CompileAssemblyFromSource(IDictionary<string, string> sources);
        CompilerResults CompileAssemblyFromSource(string modelName, string source);
    }
}