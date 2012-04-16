#region License
#endregion

using System;

using Aspid.Core.Extensions;
using Aspid.Core.Properties;

namespace Aspid.Core
{
    /// <summary>
    /// Static class with methods used to load the application initializer.
    /// </summary>
    public static class ApplicationInitializerLocator
    {
        /// <summary>
        /// Gets the currently configured application initializer.
        /// (Aspid.Core.Properties.Settings.Default.ApplicationInitializer)
        /// </summary>
        /// <returns>The currently configured application initializer</returns>
        public static IApplicationInitializer GetApplicationInitializer()
        {
            const string ERROR_NO_CONFIGURED_INITIALIZER = "There's no configured initializer";

            var initializerName = Settings.Default.ApplicationInitializer;
            if (initializerName.IsNullOrEmpty()) throw new ApplicationException(ERROR_NO_CONFIGURED_INITIALIZER);

            return GetApplicationnInitializer(initializerName);
        }

        private static IApplicationInitializer GetApplicationnInitializer(string initializerName)
        {
            const string ERROR_CANT_FIND_APPLICATION_INITIALIZER = "Public constructor was not found for {0}";

            try
            {
                var type = Type.GetType(initializerName);
                return GetApplicationnInitializer(type);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ERROR_CANT_FIND_APPLICATION_INITIALIZER.InvariantFormat(initializerName), ex);
            }
        }

        private static IApplicationInitializer GetApplicationnInitializer(Type type)
        {
            const string ERROR_NO_PUBLIC_CONSTRUCTOR = "Public constructor was not found for {0}";
            const string ERROR_TYPE_IS_NOT_APPLICATION_INITIALIZER = "{0} Type does not implement {1}";
            const string ERROR_UNABLE_TO_INSTANTIATE = "Unable to instantiate: {0}";

            try
            {
                return (IApplicationInitializer)Activator.CreateInstance(type);
            }
            catch (MissingMethodException ex)
            {
                throw new ApplicationException(ERROR_NO_PUBLIC_CONSTRUCTOR.InvariantFormat(type), ex);
            }
            catch (InvalidCastException ex)
            {
                throw new ApplicationException(ERROR_TYPE_IS_NOT_APPLICATION_INITIALIZER.InvariantFormat(type, typeof(IApplicationInitializer)), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ERROR_UNABLE_TO_INSTANTIATE.InvariantFormat(type), ex);
            }
        }
    }
}
