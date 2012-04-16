#region License
#endregion

using System;
using System.Collections.Generic;

using Microsoft.Win32;

namespace Aspid.Core.Utils
{
    public static class FrameworkUtils
    {
        public class DotNetFrameworkVersion
        {
            public string GlobalVersion { get; internal set; }
            public string FullVersion { get; internal set; }
            public int? ServicePack { get; internal set; }

            public override string ToString()
            {
                return GlobalVersion + (ServicePack != null ? String.Format(" SP{0}", ServicePack) : "");
            }
        }

        public static IEnumerable<DotNetFrameworkVersion> GetInstalledFrameworkVersions()
        {
            var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Net Framework Setup\NDP\");

            foreach (var subKeyName in ndpKey.GetSubKeyNames())
            {
                using (var subKey = ndpKey.OpenSubKey(subKeyName))
                {
                    yield return new DotNetFrameworkVersion
                    {
                        GlobalVersion = subKeyName,
                        FullVersion = (string)subKey.GetValue("Version"),
                        ServicePack = (int?)subKey.GetValue("SP")
                    };
                }
            }
        }
    }
}
