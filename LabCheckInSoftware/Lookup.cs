
using System.Windows.Documents;

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
                    if (uID.Length < 8)
                        return userInfo; //Just return the empty string if the given uID is not a valid number of characters.
                    //Change the uID to a standard form, as sometimes users enter '0' as the first char or 'u'
                    uID = 'u' + uID.Substring(1);
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

        /// <summary>
        /// First performs a lookup on a user given their ID, then changes any info for that user for any non-null parameter.
        /// Returns a bool based on success - true means success, false means failure.
        /// </summary>
        public static bool EditUserInfoGivenID(string uID, string? newID, string? firstName, string? lastName, string? infField1, string? infField2)
        {
            try
            {
                //First find where the User is located within the Users file
                string UsersFile = System.IO.File.ReadAllText(SettingsController.SaveFileLocation + "Users.csv");
                string[] users = UsersFile.Split('\n');
                //Change the uID to a standard form, as sometimes users enter '0' as the first char or 'u'
                uID = 'u' + uID.Substring(1);
                int usersRow = 0;
                string[] usersInfo = new string[6];
                foreach (string user in users)
                {
                    if (user.Contains(uID))
                    {
                        usersInfo = user.Split(',');
                        break;
                    }
                    usersRow++;

                }

                //If usersInfo is empty, the search has failed
                if (usersInfo[0].Length == 0)
                    throw new Exception();

                //Now that we've found the index for where the user's info is stored, let's change the info requested
                if (newID != null) usersInfo[0] = newID;
                if (firstName != null) usersInfo[1] = firstName;
                if (lastName != null) usersInfo[2] = lastName;
                if (infField1 != null) usersInfo[3] = infField1;
                if (infField2 != null) usersInfo[4] = infField2;

                //Info is updated, let's replace the old info in the file and save
                users[usersRow] = string.Join(',', usersInfo);
                UsersFile = string.Join('\n', users);
                System.IO.File.WriteAllText(SettingsController.SaveFileLocation + "Users.csv", UsersFile);

                return true;
            }
            catch
            {
                //If anything failed, this means that the uID given doesn't exist, return false for failure
                return false;
            }
            
        }

    }
}
