using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLogging
{
    internal static class Tracker
    {
        /// <summary>
        /// Creates an Attendance Tracker .csv file for tracking if specified given users have checked in on the specified given days
        /// of the week, during the specified given time frame. Option to make this biweekly (every other week) too.
        /// Returns whether or not the method was successful.
        /// </summary>
        public static bool CreateAttendanceTracker(DateTime fromDate, DateTime toDate, bool[] daysOfWeek, bool biweekly, string[] IDsToTrack)
        {

            return false;
        }
    }
}
