#region License
#endregion

using System;

namespace Aspid.Core
{
    public interface IApplicationInitializer
    {
        IServiceLocator Wire();

        void Dewire();
    }
}
