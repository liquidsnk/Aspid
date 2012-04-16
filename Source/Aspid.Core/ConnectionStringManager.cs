#region License
#endregion

using System;
using System.Configuration;

namespace Aspid.Core
{
    public static class ConnectionStringManager
    {
        public static string CommonConnectionStringIdentifierName { get; set; }

        static ConnectionStringManager()
        {
            CommonConnectionStringIdentifierName = "COMMON_DATA_SOURCE";
        }

        public static string GetConnectionStringForMachine()
        {
            //try to get the specific connection string configured for this machine
            var connectionString = ConfigurationManager.ConnectionStrings[Environment.MachineName];
            if (connectionString == null)
            {
                //can't find a connection string for this machine, search for "COMMON_DATA_SOURCE"
                connectionString = ConfigurationManager.ConnectionStrings[CommonConnectionStringIdentifierName];
            }

            return connectionString == null ? null : connectionString.ConnectionString;
        }
    }
}
