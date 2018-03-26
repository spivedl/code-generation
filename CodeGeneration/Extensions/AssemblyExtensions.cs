using System.Reflection;

namespace CodeGeneration.Extensions
{
    // TODO: Rename this class to something more appropriate
    public static class AssemblyExtensions
    {
        public static string Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public static string Author()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return System.IO.File.GetAccessControl(assemblyLocation)
                .GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
        }
    }
}
