using System;
using System.Collections.Generic;

namespace CodeGeneration.Services.Template
{
    public interface ITemplateService
    {
        IEnumerable<string> GetEmbeddedTemplateNames();
        IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths);
        IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths, IEnumerable<string> filterByNames);
        IEnumerable<string> GetEmbeddedTemplateNames(string filterByPath);
        string ResolveTemplate(string templateName);
        string Process(string templateName, object model = null, Type modelType = null);
    }
}