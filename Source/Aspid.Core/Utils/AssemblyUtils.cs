#region License
#endregion

using System;
using System.Linq;
using System.IO;
using System.Reflection;

using Aspid.Core.Extensions;

namespace Aspid.Core.Utils
{
    /// <summary>
    /// Static utility methods that work on assemblies.
    /// </summary>
    public static class AssemblyUtils
    {
        /// <summary>
        /// Gets the embedded resource for an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns></returns>
        public static string GetEmbeddedResource(Assembly assembly, string fileName)
        {
            string embeddedResourceName = GetResourceWhoseNameEndsWith(assembly, fileName);

            //Resource name not found
            if (string.IsNullOrEmpty(embeddedResourceName))
            {
                string nameNotFoundError = String.Format("Could not locate embedded resource {0} in assembly {1}", fileName, assembly.GetName().Name);
                throw new FileNotFoundException(nameNotFoundError, fileName);
            }

            try
            {
                return GetEmbeddedResourceByExactName(assembly, embeddedResourceName);
            }
            catch (IOException ex)
            {
                string fileNotFoundError = String.Format("File not found in assembly {1}", assembly.GetName().Name);
                throw new FileNotFoundException(fileNotFoundError, embeddedResourceName, ex);
            }
        }

        private static string GetEmbeddedResourceByExactName(Assembly assembly, string embeddedResourceName)
        {
            var stream = assembly.GetManifestResourceStream(embeddedResourceName);

            //Read the contents of the embedded file
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetResourceWhoseNameEndsWith(Assembly assembly, string resourceName)
        {
            //Resources are named using a fully qualified name
            var resourceNames = assembly.GetManifestResourceNames();

            //Return the first resource name that ends with the given fileName
            return resourceNames.FirstOrDefault(x => x.EndsWith(resourceName.SafeTrim()));
        }
    }
}