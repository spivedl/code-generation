using System;

namespace CodeGeneration.Services.Template
{
    public interface ITemplateService
    {
        string Process(string templateName, object model = null, Type modelType = null);
    }
}