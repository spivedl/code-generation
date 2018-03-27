using System.Collections;
using System.Collections.Generic;

namespace CodeGeneration.Services.Generation
{
    public interface IGeneratorService<in T>
    {
        void Generate(T context);
        IDictionary<string, IDictionary<string, string>> GetCache();
        string GetCachedResult(string modelName, string templateName);
        string GetTemplate(string templateName);
    }
}