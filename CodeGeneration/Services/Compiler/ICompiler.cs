using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CodeGeneration.Services.Compiler
{
    public interface ICompiler
    {
        IEnumerable<CompilerResults> CompileAssemblyFromSource(IEnumerable<string> sources);
    }
}