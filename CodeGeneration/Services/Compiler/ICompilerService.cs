using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeGeneration.Services.Compiler
{
    public interface ICompilerService
    {
        IEnumerable<CompilerResults> CompileAssemblyFromSource(IEnumerable<string> sources);
        CompilerResults CompileAssemblyFromSource(string source);
    }
}