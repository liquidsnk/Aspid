#region License
#endregion

using System.Reflection;

using Aspid.Core.Utils;

namespace Aspid.Core.Extensions
{
    /// <summary>
    /// Extension methods for Assembly class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the embedded resource for an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetEmbeddedResource(this Assembly assembly, string fileName)
        {
            return AssemblyUtils.GetEmbeddedResource(assembly, fileName);
        }

        /// <summary>
        /// Gets the first resource whose resource name ends with the provided string.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fileName">Resource name/suffix.</param>
        /// <returns></returns>
        public static string GetEmbeddedResourceName(this Assembly assembly, string fileName)
        {
            return AssemblyUtils.GetResourceWhoseNameEndsWith(assembly, fileName);
        }
    }
}
