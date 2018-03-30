using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeGeneration.Services.Cache;
using Microsoft.CSharp;
using NLog;

namespace CodeGeneration.Services.Compiler
{
    public class CSharpInMemoryCompiler : ICompilerService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly CSharpCodeProvider _codeProvider;
        private readonly CompilerParameters _compilerParameters;
        private readonly ICacheService _cacheService;

        public CSharpInMemoryCompiler(ICacheService cacheService)
        {
            _cacheService = cacheService;

            _codeProvider = new CSharpCodeProvider();
            _compilerParameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                OutputAssembly = "CodeGeneration.Models.Domain.Models.dll",
                ReferencedAssemblies = { "mscorlib.dll","System.dll"}
            };

            // add reference to the current assembly (CodeGeneration) so we can use things like CodeGeneration.Models.Domain.Attributes in our templates
            _compilerParameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
        }

        public IDictionary<string, Type> GetCompiledTypeFromSource(IDictionary<string, string> sources)
        {
            var dictionary = new Dictionary<string, Type>();

            foreach (var kvp in sources)
            {
                var modelName = kvp.Key;
                var source = kvp.Value;

                if (dictionary.ContainsKey(modelName)) continue;

                dictionary.Add(modelName, GetCompiledTypeFromSource(modelName, source));
            }

            return dictionary;
        }

        public Type GetCompiledTypeFromSource(string modelName, string source)
        {
            var compilerResults = CompileAssemblyFromSource(modelName, source);
            return compilerResults.CompiledAssembly.GetExportedTypes().FirstOrDefault();
        }

        public IDictionary<string, CompilerResults> CompileAssemblyFromSource(IDictionary<string, string> sources)
        {
            var dictionary = new Dictionary<string, CompilerResults>();

            foreach (var kvp in sources)
            {
                var modelName = kvp.Key;
                var source = kvp.Value;

                if (dictionary.ContainsKey(modelName)) continue;

                dictionary.Add(modelName, CompileAssemblyFromSource(modelName, source));
            }

            return dictionary;
        }

        public CompilerResults CompileAssemblyFromSource(string modelName, string source)
        {
            CompilerResults compilerResults;

            if (_cacheService.Exists(modelName))
            {
                Logger.Info("[CACHE HIT]: CompilerResults for {0} found in cache.", modelName);
                compilerResults = _cacheService.Get<CompilerResults>(modelName);
            }
            else
            {
                Logger.Info("[CACHE MISS]: CompilerResults for {0} NOT found in cache. Will compile source code in memory and add results to cache.", modelName);

                try
                {
                    compilerResults = _codeProvider.CompileAssemblyFromSource(_compilerParameters, source);

                    if (compilerResults.Errors.HasErrors) throw new CompilerErrorException($"Exception when attempting to compile source code for modelName '{modelName}'.")
                    {
                        ModelName = modelName,
                        SourceCode = source,
                        Errors = compilerResults.Errors
                    };

                    _cacheService.Set(modelName, compilerResults);

                }
                catch (Exception ex)
                {
                    if (ex is CompilerErrorException ceException)
                    {
                        Logger.Error(ceException.Message);
                        Logger.Error(ceException.ModelName);
                        Logger.Error(ceException.SourceCode);

                        foreach (CompilerError compilerError in ceException.Errors)
                        {
                            Logger.Error("Line {0}: {1}", compilerError.Line, compilerError.ErrorText);
                        }
                    }
                    else
                    {
                        Logger.Error(ex);
                    }

                    throw;
                }
            }

            return compilerResults;
        }
    }
}