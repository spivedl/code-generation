﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using NLog;

namespace CodeGeneration.Services.Compiler
{
    public class CSharpInMemoryCompiler : ICompilerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CSharpCodeProvider _codeProvider;
        private readonly CompilerParameters _compilerParameters;

        public CSharpInMemoryCompiler()
        {
            _codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            _compilerParameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                OutputAssembly = typeof(CSharpInMemoryCompiler).Assembly.FullName
            };

            _compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
            _compilerParameters.ReferencedAssemblies.Add("System.dll");
        }

        public IEnumerable<CompilerResults> CompileAssemblyFromSource(IEnumerable<string> sources)
        {
            return sources.Select(sourceCode => _codeProvider.CompileAssemblyFromSource(_compilerParameters, sourceCode)).ToList();
        }

        public CompilerResults CompileAssemblyFromSource(string source)
        {
            return _codeProvider.CompileAssemblyFromSource(_compilerParameters, source);
        }
    }
}