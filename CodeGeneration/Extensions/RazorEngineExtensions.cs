using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace CodeGeneration.Extensions
{
    public static class RazorEngineExtensions
    {
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static readonly string AssemblyName = ExecutingAssembly.GetName().Name;
        private const string FileExtension = "cshtml";

        public static string Version()
        {
            return ExecutingAssembly.GetName().Version.ToString();
        }

        public static string Author()
        {
            return File
                .GetAccessControl(ExecutingAssembly.Location)
                .GetOwner(typeof(NTAccount))
                .ToString();
        }

        public static string ToRazorEngineKey(this string resourceName, string templateDirectory = "")
        {
            var parts = resourceName.ToEmbeddedResourceParts();

            // if the resource name has more than 1 part
            // resourceName is assummed to be in this format: AssemblyName.RootDirectory.Directory1.DirectoryN.FileName.FileExtension
            if (parts.Length > 1) return string.Join(".", parts);

            // if the resource name has only 1 part
            // a template directory must be supplied separately, otherwise we have no way of knowing which directoy the template is in
            // e.g. there could be a Create template in multiple directories, so we need to know the template directory before we can construct the RazorEngine Key
            templateDirectory = templateDirectory.RemoveAssemblyName().RemoveFileExtension();
            if (string.IsNullOrWhiteSpace(templateDirectory)) throw new Exception("Template directory cannot be empty when passing only a View name to the 'ToRazorEngineKey' extension method!");

            return $"{templateDirectory}.{parts.Last()}";
        }

        public static string ToFileName(this string resourceName)
        {
            var parts = resourceName.ToEmbeddedResourceParts();
            return $"{parts.Last()}.{FileExtension}";
        }

        public static string ToTemplateName(this string resourceName)
        {
            var parts = resourceName.ToEmbeddedResourceParts();
            return parts.Last();
        }

        public static string[] ToEmbeddedResourceParts(this string input, char[] delimiters = null)
        {
            if (delimiters == null) delimiters = new []{'.'};

            return input
                .RemoveAssemblyName()
                .RemoveFileExtension()
                .Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string RemoveAssemblyName(this string input, string assemblyName = null)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) assemblyName = AssemblyName;

            return input.Replace(assemblyName, "");
        }

        public static string RemoveFileExtension(this string input, string fileExtension = FileExtension)
        {
            return input.Replace(fileExtension, "");
        }
    }
}
