using System;
using System.Collections.Generic;
using System.IO;
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

        using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace LabManageSystemFix
    {
        internal class Program
        {
            static void Main(string[] args)
            {
                List<Dictionary<string, Dictionary<DateTime, bool>>> attendanceTrackers = new();
                string[] attTrackerPaths = {
            "C:\\ProgramData\\TWLogging\\AttendanceTrackers\\MEEN4000-002.csv",
            "C:\\ProgramData\\TWLogging\\AttendanceTrackers\\MEEN4000-003.csv",
            "C:\\ProgramData\\TWLogging\\AttendanceTrackers\\MEEN4010-002.csv",
            "C:\\ProgramData\\TWLogging\\AttendanceTrackers\\MEEN4010-003.csv",
            "C:\\ProgramData\\TWLogging\\AttendanceTrackers\\MEEN4010-004.csv" };
                for (int i = 0; i < attTrackerPaths.Count(); i++)
                {
                    Dictionary<string, Dictionary<DateTime, bool>> info = new();
                    using (var reader = new StreamReader(attTrackerPaths[i]))
                    {
                        string s;
                        string name = reader.ReadLine()!;
                        while ((s = reader.ReadLine()!) != null)
                        {
                            info.Add(s.Substring(0, s.IndexOf(",")), new Dictionary<DateTime, bool>());
                        }
                        info.Add("u1313191", new Dictionary<DateTime, bool>());
                        attendanceTrackers.Add(info);
                    }
                }

                DateTime startDate = new DateTime(2025, 1, 6);
                DateTime endDate = new DateTime(2025, 4, 17);
                DateTime currDate = startDate;
                Dictionary<string, Dictionary<DateTime, bool>> studentInfo = new();
                while (currDate != endDate)
                {
                    string csvFilePath = "C:\\ProgramData\\TWLogging\\Logs\\" + currDate.ToString("yyyy-MMMM") + "\\log" + currDate.ToString("M-d-yyyy") + ".csv";
                    try
                    {
                        using (var reader = new StreamReader(csvFilePath))
                        {
                            string s;
                            while ((s = reader.ReadLine()!) != null)
                            {
                                //Console.WriteLine(s.Substring(0, s.IndexOf(",")));
                                s = s.Substring(0, s.IndexOf(","));
                                if (!studentInfo.ContainsKey(s))
                                    studentInfo.Add(s, new Dictionary<DateTime, bool>());
                                studentInfo[s].Add(currDate, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                    //Console.WriteLine(currDate.ToString("M-d-yyyy"));
                    currDate = currDate.AddDays(1);
                }

                for (int i = 0; i < attTrackerPaths.Count(); i++)
                {
                    Dictionary<string, Dictionary<DateTime, bool>> info = new();
                    using (var reader = new StreamReader(attTrackerPaths[i]))
                    {
                        string s;
                        string firstRow = reader.ReadLine()!;
                        string[] Dates = firstRow.Split(',');
                        using (StreamWriter writer = File.CreateText(attTrackerPaths[i].Substring(0, attTrackerPaths[i].Length - 4) + "(1).csv"))
                        {
                            writer.WriteLine(firstRow);
                            for (int j = 0; j < attendanceTrackers.Count(); j++)
                            {
                                foreach (string uID in attendanceTrackers[j].Keys)
                                {
                                    string newLine = "";
                                    int abs = 0;
                                    bool studentExists = true;
                                    for (int k = 2; k < Dates.Length; k++)
                                    {
                                        DateTime date = DateTime.Parse(Dates[k]);
                                        if (date > endDate)
                                            break;
                                        try
                                        {
                                            if (studentInfo[uID].ContainsKey(date))
                                            {
                                                newLine += "yes,";
                                            }
                                            else { newLine += "no,"; abs++; }
                                        }
                                        catch
                                        {
                                            //This student doesn't exist in any logs so just don't put their uID in
                                            studentExists = false;
                                        }

                                    }
                                    if (studentExists)
                                        writer.WriteLine(uID + "," + abs.ToString() + "," + newLine);
                                }
                            }
                        }

                    }
                }

            }
        }
    }

}
}
