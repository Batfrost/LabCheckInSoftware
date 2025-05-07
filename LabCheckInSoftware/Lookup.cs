using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLogging
{
    /// <summary>
    /// This class covers the logic over searching through and editing the Users.csv file and the log files.
    /// </summary>
    internal static class Lookup
    {
        /// <summary>
        /// Examines the Users.csv file for a specific user, either by uID or by name, and returns a string array of
        /// the user's: uID, first name, last name, the two custom info fields, and then the date they first registered.
        /// If the user isn't found, returns a blank string array.
        /// </summary>
        public static string[] LookupUser(string? uID, string? firstName, string? lastName)
        {
            string[] userInfo = new string[6];
            try
            {
                string UsersFile = System.IO.File.ReadAllText(SettingsController.SaveFileLocation + "Users.csv");
                string[] users = UsersFile.Split('\n');
                //If uID isn't null, search by uID
                if (uID != null)
                {
                    foreach (string user in users)
                    {
                        if (user.Contains(uID))
                        {
                            return user.Split(',');
                        }
                    }
                }
                //Search by name if uID is null
                else
                {
                    foreach (string user in users)
                    {
                        if (user.Contains(firstName!) && user.Contains(lastName!))
                        {
                            return user.Split(',');
                        }
                    }
                }

                return userInfo;
            }
            catch
            {
                return userInfo;
            }
                
        }

        /// <summary>
        /// Examines the log file from the given log file path and returns either the row number of the log that
        /// the user is on (with zero-indexing), or a -1 if the user wasn't found in the log file.
        /// </summary>
        public static int LookupUserInLogFile(string logFilePath, string uID)
        {
            string log = System.IO.File.ReadAllText(logFilePath);
            string[] users = log.Split("\n");
            int index = 0;
            foreach (string user in users)
            {
                if (user.Contains(uID))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

    }
}
