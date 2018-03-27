using System;
using System.Collections.Generic;
using RazorEngine.Templating;

namespace CodeGeneration.Services.Template.Razor
{
    public interface IRazorTemplateService : ITemplateService
    {
        IEnumerable<string> GetEmbeddedTemplateNames();
        IEnumerable<string> GetEmbeddedTemplateNames(string path);
        string ResolveTemplate(string templateName);
        string Process(string templateName, Type modelType = null, object model = null, DynamicViewBag viewBag = null);
    }
}