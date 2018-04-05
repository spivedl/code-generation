using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Services.Cache;
using NLog;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace CodeGeneration.Services.Template.Razor
{
    // TODO: Add logging to the RazorTemplateService
    public class RazorTemplateService : IRazorTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEnumerable<string> _embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        private readonly ApplicationOptions _applicationOptions;
        private readonly ITemplateManager _templateManager;
        private readonly IRazorEngineService _razorEngine;

        public RazorTemplateService(ApplicationOptions applicationOptions)
        {
            _applicationOptions = applicationOptions;

            _templateManager = new EmbeddedResourceTemplateManager(typeof(Program));
            ITemplateServiceConfiguration razorConfiguration = new TemplateServiceConfiguration
            {
                TemplateManager = _templateManager,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            _razorEngine = RazorEngineService.Create(razorConfiguration);
        }

        public IEnumerable<string> GetEmbeddedTemplateNames()
        {
            return _embeddedResources;
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths)
        {
            var filterByPathList = filterByPaths.ToList();
            if (!filterByPathList.Any()) return _embeddedResources;

            return _embeddedResources.Where(er =>
            {
                var directory = GetDirectoryName(er);

                return filterByPathList.Contains(directory);
            });
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths, IEnumerable<string> filterByNames)
        {
            var filterByPathList = filterByPaths.ToList();
            var filterByNameList = filterByNames.ToList();
            var filteredResourceNames = _embeddedResources;

            if (filterByPathList.Any())
            {
                filteredResourceNames = _embeddedResources.Where(er =>
                {
                    var directory = GetDirectoryName(er);

                    return filterByPathList.Contains(directory);
                });
            }

            if (filterByNameList.Any())
            {
                filteredResourceNames = filteredResourceNames.Where(er =>
                {
                    var file = GetFileName(er);

                    return filterByNameList.Contains(file);
                });
            }

            return filteredResourceNames.ToList();
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(string filterByPath)
        {
            if (string.IsNullOrWhiteSpace(filterByPath)) return _embeddedResources;

            return _embeddedResources.Where(er =>
            {
                var directory = GetDirectoryName(er);

                return filterByPath.Equals(directory);
            });
        }

        public string ResolveTemplate(string templateName)
        {
            try
            {
                var templateKey = _razorEngine.GetKey(templateName);
                var templateSource = _templateManager.Resolve(templateKey);

                return templateSource.Template;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error resolving template for name '{templateName}'!");
                return string.Empty;
            }
        }

        public string Process(string templateName, object model = null, Type modelType = null)
        {
            return _razorEngine.RunCompile(templateName, modelType, model);
        }

        public string Process(string templateName, Type modelType = null, object model = null, DynamicViewBag viewBag = null)
        {
            return _razorEngine.RunCompile(templateName, modelType, model, viewBag);
        }

        private string GetDirectoryName(string resourceName)
        {
            // after formatting resource name should only include non-root directories and the resource name
            // e.g. Namespace.RootDirectory.DirectoryA.DirectoryB.FileName.Extension => DirectoryA.DirectoryB.FileName
            var parts = FormatResourceNameString(resourceName);

            return parts.Count > 2 
                ? string.Join(".", parts.Take(parts.Count - 2)) 
                : parts.First();
        }

        private string GetFileName(string resourceName)
        {
            // after formatting resource name should only include non-root directories and the resource name
            // e.g. Namespace.RootDirectory.DirectoryA.DirectoryB.FileName.Extension => DirectoryA.DirectoryB.FileName
            var parts = FormatResourceNameString(resourceName);

            return parts.Last();
        }

        private List<string> FormatResourceNameString(string resourceName)
        {
            var parts = resourceName.Split('.').ToList();

            // replace any usages of the namespace of the application 
            var namespaceName = typeof(Program).Namespace ?? "";
            parts.Remove(namespaceName);

            // remove any usages of the cshtml file extension
            parts.Remove("cshtml");

            // remove any usages of "RootTemplateDirectories" specified in the application options
            foreach (var rootTemplateDirectory in _applicationOptions.RootTemplateDirectories)
            {
                parts.Remove(rootTemplateDirectory);
            }

            if (parts.Count < 2) throw new Exception($"Unknown embedded resource format: '{resourceName}'!");

            return parts;
        }
    }
}