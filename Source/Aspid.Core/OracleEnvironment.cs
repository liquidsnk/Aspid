using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    /// <summary>
    /// Helper functions for working with ORACLE enviroment
    /// </summary>
    class OracleEnvironment
    {
        private const string SOFTWARE_ORACLE_ALL_HOMES = "SOFTWARE\\ORACLE\\ALL_HOMES";
        private const string ORACLE_LAST_HOME = "LAST_HOME";
        private const string ORACLE_ACTUAL_HOME = "SOFTWARE\\ORACLE\\HOME";
        private const string ORACLE_HOME = "ORACLE_HOME";

        public static IEnumerable<string> possibleTnsLocations = new List<string>
                                                                 {
                                                                     "\\NETWORK\\ADMIN\\TNSNAMES.ORA",
                                                                     "\\NET80\\ADMIN\\TNSNAMES.ORA"
                                                                 };

        /// <summary>
        /// Gets the TNS names.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetTnsNames()
        {
            List<string> tnsNamesList = new List<string>();
            string tnsNamePattern = @"[\n][\s]*[^\(][a-zA-Z0-9_.]+[\s]*=[\s]*\(";

            string tnsPath = GetTnsNamesFilePath();
            if (!String.IsNullOrEmpty(tnsPath))
            {
                //check out that file does physically exists
                FileInfo tnsFile = new FileInfo(tnsPath);
                if ((tnsFile.Exists) && (tnsFile.Length > 0))
                {
                    //read tnsnames.ora file
                    foreach (Match match in Regex.Matches(File.ReadAllText(tnsFile.FullName), tnsNamePattern))
                    {
                        tnsNamesList.Add(match.Value.Trim().SafeRemove(match.Value.Trim().IndexOf(" ", StringComparison.OrdinalIgnoreCase)));
                    }
                }
            }

            return tnsNamesList;
        }

        /// <summary>
        /// Gets the home directory path.
        /// </summary>
        /// <returns></returns>
        private static string GetHomeDirectoryPath()
        {
            string homePath = string.Empty;
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey allHomeKey = localMachineKey.OpenSubKey(SOFTWARE_ORACLE_ALL_HOMES);

            if (!String.IsNullOrEmpty(localMachineKey.ToStringOrEmpty()))
            {
                string lastHome = allHomeKey.GetValue(ORACLE_LAST_HOME).ToString();
                RegistryKey actualHomeKey = Registry.LocalMachine.OpenSubKey(ORACLE_ACTUAL_HOME + lastHome);
                homePath = actualHomeKey.GetValue(ORACLE_HOME).ToString();
            }

            return homePath;
        }

        /// <summary>
        /// Gets the TNS names file path.
        /// </summary>
        /// <returns></returns>
        private static string GetTnsNamesFilePath()
        {
            string filePath;

            string homeDirectory = GetHomeDirectoryPath();
            if (!String.IsNullOrEmpty(homeDirectory))
                filePath = FindTnsFilePath(homeDirectory);
            else
                filePath = "";

            return filePath;
        }

        /// <summary>
        /// Finds the TNS file path.
        /// </summary>
        /// <param name="homeDirectory">The home directory.</param>
        /// <returns></returns>
        private static string FindTnsFilePath(string homeDirectory)
        {
            foreach (var location in possibleTnsLocations)
            {
                string tentativeLocation = homeDirectory + location;
                if (File.Exists(tentativeLocation))
                {
                    return tentativeLocation;
                }
            }

            return string.Empty;
        }                   
    }
}
