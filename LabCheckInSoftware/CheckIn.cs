
namespace TWLogging
{
    /// <summary>
    /// This class covers all the logic for checking a user into a daily log file.
    /// </summary>
    public static class CheckIn
    {
        /// <summary>
        /// Attempts to get information about this specific user to display from a Users.csv file,
        /// where if the user doesn't exist in that file returns null.
        /// If the user does exist within the file, the method will then call CheckInToFile() and
        /// then return the name of the user.
        /// </summary>
        public static string? AttemptCheckIn(string uID)
        {
            string[] userInfo = Lookup.LookupUser(uID, null, null);
            //Check if user exists or not
            if (userInfo[0] is null)
                return null;

            CheckInToFile(userInfo);

            //Return the name of the user: First Last
            return userInfo[1] + " " + userInfo[2];
        }

        //Private helper that creates a new daily log if one doesn't already exist and 
        //checks in the user to that log file with the current time, or adds a new time
        //onto the end of the checked-in user's row if the user has already checked in on this date.
        private static void CheckInToFile(string[] userInfo)
        {
            //Check if today's log file exists, and if not, create it
            string logFilePath = SettingsController.SaveFileLocation + "Logs\\" + DateTime.Now.ToString("yyyy-MMMM") + "\\log" + DateTime.Today.ToString("MM-dd-yyyy") + ".csv";
            if (!System.IO.Directory.Exists(logFilePath)) //Creating the directory too
                System.IO.Directory.CreateDirectory(logFilePath.Substring(0, logFilePath.LastIndexOf('\\')));
            try
            {
                string log = System.IO.File.ReadAllText(logFilePath);
                //The daily log already exists, time to check if the user has already made a log today
                int rowIndex = Lookup.LookupUserInFile(logFilePath, userInfo[0]);

                //If the value for usersRow is -1, this user doesn't exist in the log file and can just be added as a new row onto the end
                if (rowIndex == -1)
                    System.IO.File.WriteAllText(logFilePath, log + userInfo[0] + "," + userInfo[1] + "," + userInfo[2] + "," + userInfo[3] + "," + userInfo[4] + "," + DateTime.Now.ToString("t") + "\n"); //"t" is code for short time: e.g. 1:30 PM
                else
                {
                    //The log exists for today, and the user has already checked in at least once today too, so on the row this user has been checked in on already, just add a new check-in-time to the end
                    string[] rows = log.Split('\n');
                    string usersRow = rows[rowIndex].Trim('\n') + "," + DateTime.Now.ToString("t") + "\n";
                    string beforeUsersRow = string.Join('\n', rows.Take(rowIndex).ToArray());
                    string afterUsersRow = string.Join('\n',rows.Skip(rowIndex+1).ToArray());
                    string editedLog = beforeUsersRow + usersRow + afterUsersRow;
                    System.IO.File.WriteAllText(logFilePath, editedLog);
                }

            }
            catch { System.IO.File.WriteAllText(logFilePath, userInfo[0] + "," + userInfo[1] + "," + userInfo[2] + "," + userInfo[3] + "," + userInfo[4] + DateTime.Now.ToString("t") + "\n"); }
        }
    }
}
