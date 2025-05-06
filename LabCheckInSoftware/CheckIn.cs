using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLogging
{
    public static class CheckIn
    {
        /// <summary>
        /// Attempts to get information about this specific user to display from a Users.csv file,
        /// where if the user doesn't exist in that file, or the file doesn't exist, returns null.
        /// If the user does exist within the file, the method will then call CheckInToFile() and
        /// then return the name of the user.
        /// </summary>
        /// <param name="ID">The uID of the user attempting to check in</param>
        /// <returns>Name of user if successful, or null if unsuccessful</returns>
        public static string? AttemptCheckIn(string ID)
        {


            return null;
        }
    }
}
