﻿using System;
using CodeGeneration.Models.Configuration;
using DotLiquid;
using DotLiquid.NamingConventions;
using NLog;

namespace CodeGeneration.Services.Template.Liquid
{
    public class LiquidTemplateService : BaseTemplateService, ILiquidTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LiquidTemplateService(ApplicationOptions applicationOptions) : base(applicationOptions, Logger)
        {
            DotLiquid.Template.NamingConvention = new CSharpNamingConvention();
        }

        public override string Process(string templateName, object model = null, Type modelType = null)
        {
            var templateContent = ResolveTemplate(templateName);
            var template = DotLiquid.Template.Parse(templateContent);

            return template.Render(Hash.FromAnonymousObject(model, includeBaseClassProperties: true));
        }
    }
}
