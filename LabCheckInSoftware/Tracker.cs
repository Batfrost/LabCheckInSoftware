using System.IO;

namespace TWLogging
{
    /// <summary>
    /// This class will handle most of the Attendance Tracker Logic, such as creating or updating a tracker.
    /// </summary>
    internal static class Tracker
    {
        /// <summary>
        /// Creates an Attendance Tracker .csv file for tracking if specified given users have checked in on the specified given days
        /// of the week, during the specified given time frame. Option to make this biweekly (every other week) too.
        /// Returns whether or not the method was successful.
        /// </summary>
        public static bool CreateAttendanceTracker(string NameOfTracker, DateTime fromDate, DateTime toDate, bool[] daysOfWeek, bool biweekly, string[] IDsToTrack)
        {
            try
            {
                //First, the top row will be made, containing the name, absence count, and all the dates from the given information
                System.Text.StringBuilder tracker = new();
                tracker.Append(NameOfTracker + ",Absences");

                DateTime currDate = fromDate;
                int weekCount = 0; //This will be used to keep track of biweekly weeks, if that option was checked
                while (currDate <= toDate)
                {
                    //Skip this date if it is a Saturday or Sunday, then also check if the biweekly option is enabled and whether or not this week falls in that category
                    if (currDate.DayOfWeek != DayOfWeek.Saturday && currDate.DayOfWeek != DayOfWeek.Sunday && (!biweekly || (biweekly && weekCount % 2 == 0)))
                        //In daysOfWeek, [0] represents Monday, but Monday in integer format for a DateTime object is 1, so subtract 1
                        if (daysOfWeek[(int)currDate.DayOfWeek - 1])
                            //This is on a valid day of the week, so add this date to the tracker
                            tracker.Append(',' + currDate.ToString("M-d-yyyy"));

                    currDate = currDate.AddDays(1);
                    if (currDate.DayOfWeek == DayOfWeek.Monday)
                        weekCount++;
                }
                tracker.Append('\n');

                //First Row of .csv tracker is finished, time to add rows for each ID to track, and if the from date < current date, check logs and track attendance for any days previous to today
                if (fromDate <= DateTime.Today)
                {
                    DateTime endDate = (DateTime.Today < toDate) ? DateTime.Today : toDate; //Only loop to whichever is sooner: the to date, or the current date - for inner loop that goes over dates
                    //Now, for each ID to track, we will check each valid date's log and mark whether or not they checked in that date or not
                    for (int i = 0; i < IDsToTrack.Length; i++)
                    {
                        System.Text.StringBuilder newRow = new();
                        string userID = 'u' + IDsToTrack[i].Substring(1);
                        newRow.Append(userID);
                        System.Text.StringBuilder secondPart = new();
                        
                        int absenceCount = 0;
                        currDate = fromDate;
                        weekCount = 0;
                        while (currDate <= endDate)
                        {
                            //Double check that the day getting checked is valid for the specified range of dates
                            if (currDate.DayOfWeek != DayOfWeek.Saturday && currDate.DayOfWeek != DayOfWeek.Sunday && (!biweekly || (biweekly && weekCount % 2 == 0)))
                            {
                                //Now the specific log for that day will will attempt to load
                                try
                                {
                                    string csvFilePath = SettingsController.SaveFileLocation + "Logs\\" + currDate.ToString("yyyy-MMMM") + "\\log" + currDate.ToString("MM-dd-yyyy") + ".csv";
                                    //This method returns a -1 if the user wasn't found, or the row the user was found on, but this method won't require the row
                                    int foundUser = Lookup.LookupUserInFile(csvFilePath, userID);

                                    //If we didn't find that user's ID in that log, they will be marked 'no' for that date and their absence will be incremented
                                    //But only as long as the date getting checked isn't today, as that user may check in today 
                                    if (foundUser == -1 && currDate != DateTime.Today)
                                    {
                                        secondPart.Append(",no");
                                        absenceCount++;
                                    }
                                    else if (foundUser != -1) { secondPart.Append(",yes"); }
                                }
                                catch
                                {
                                    //This log was unable to open, meaning that no users from the IDsToTrack checked in this day, so this date will be marked simply as 'Holiday?'
                                    //Or if it is today, then we won't mark anything, in case people will be soon marking attendance
                                    if (currDate !=  DateTime.Today)
                                        secondPart.Append(",Holiday?");
                                }
                            }
                            currDate = currDate.AddDays(1);
                            if (currDate.DayOfWeek == DayOfWeek.Monday)
                                weekCount++;
                            //Settings.saveFileLocation + "Logs\\" + DayToCheck.ToString("yyyy-MMMM") + "\\log" + DayToCheck.ToString().Split(" ").First().Replace("/", "-") + ".csv"
                        }
                        newRow.Append(',' + absenceCount.ToString() + secondPart.ToString() + '\n');
                        tracker.Append(newRow);
                    }
                }
                else
                {
                    //The from Date is in the future, so the new rows for each ID will just be the ID + ',' + 0 for the absence count, since we don't have that future info yet
                    for (int i = 0; i < IDsToTrack.Length; i++)
                        tracker.Append(IDsToTrack[i] + ",0\n");
                }

                //Now we save the Tracker file within the Attendance Trackers folder (which will be created if it doesn't exist yet) as a .csv file
                if (!System.IO.Directory.Exists(SettingsController.SaveFileLocation + "\\Attendance Trackers"))
                    System.IO.Directory.CreateDirectory(SettingsController.SaveFileLocation + "\\Attendance Trackers");
                System.IO.File.WriteAllText(SettingsController.SaveFileLocation + "\\Attendance Trackers\\" + NameOfTracker + ".csv", tracker.ToString());
                return true;
            }
            catch
            {
                //If any errors have occured, return false, method was not successful
                //Most likely errors are that the fromDate was after the toDate, or an invalid NameOfTracker or ID in IDsToTrack was inputted
                return false;
            }
        }

        /// <summary>
        /// Checks all available attendance trackers for if this user exists within, and if they do, mark attendance for them
        /// </summary>
        public static void CheckAttendanceForUser(string uID)
        {
            uID = 'u' + uID.Substring(1);
            string[] trackerFiles = Directory.GetFiles(SettingsController.SaveFileLocation + "\\Attendance Trackers", "*", SearchOption.AllDirectories);
            foreach (string trackerFile in trackerFiles)
            {
                //Get the row the user is on in the tracker, if -1 then this user isn't in this tracker
                int userRow = Lookup.LookupUserInFile(trackerFile, uID);
                if (userRow == -1)
                    continue;

                
                string tracker = System.IO.File.ReadAllText(trackerFile);
                string[] trackerContents = tracker.Split('\n');

                //Figure out if today is a valid day for them to check in for attendance
                string[] firstRow = trackerContents[0].Split(',');
                for (int i = 2; i < firstRow.Length; i++)
                {
                    if (DateTime.Parse(firstRow[i]) == DateTime.Today)
                    {
                        //Now figure out if this user needs to be marked as here or if they already have been
                        List<string> rowInfo = trackerContents[userRow].Split(',').ToList();
                        List<string> newRow = new List<string>(rowInfo);
                        for (int j = rowInfo.Count; j <= i; j++)
                        {
                            if (j == i)
                                newRow.Add("yes");
                            else
                            {
                                //If there is more than just today that needs attendance to be marked, that means this user was absent the previous times
                                newRow[1] = (int.Parse(newRow[1]) + 1).ToString();
                                newRow.Add("no");
                            }
                        }
                        //Now to rejoin the row and all rows together and save
                        trackerContents[userRow] = string.Join(',', newRow);
                        tracker = string.Join('\n', trackerContents);
                        System.IO.File.WriteAllText(trackerFile, tracker);
                        break;
                    }
                    //If the date getting checked for the first row header is after today, then today isn't explicitly marked on the tracker
                    if (DateTime.Parse(firstRow[i]) > DateTime.Today)
                        break;
                }
            }
        }

        /// <summary>
        /// Goes through all available attendance trackers, and marks absences for users who have not logged in during this time
        /// </summary>
        public static void MarkAbsences()
        {

        }

    }
}
