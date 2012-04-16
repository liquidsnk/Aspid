#region License
#endregion

using System;

namespace Aspid.Core
{
    public interface IServiceLocator
    {
        T GetService<T>();

        T GetService<T>(string contextName);
    }
}
