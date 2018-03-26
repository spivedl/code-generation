using System;
using RazorEngine.Templating;

namespace CodeGeneration.Services.Template.Razor
{
    public interface IRazorTemplateService : ITemplateService
    {
        string Process(string templateName, Type modelType = null, object model = null, DynamicViewBag viewBag = null);
    }
}