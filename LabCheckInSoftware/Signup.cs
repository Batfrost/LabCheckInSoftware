using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLogging
{
    /// <summary>
    /// This class handles getting a new user's information and marking them down in the system
    /// </summary>
    internal static class Signup
    {
        /// <summary>
        /// Adds the given parameters of information about the new user into a Users.csv file.
        /// If the file doesn't exist, calls helper method to make one.
        /// </summary>
        public static void AddNewUser(string uID, string fName, string lName, string? infoField1, string? infoField2)
        {
            //First, get the UsersFile info, and if the file doesn't exist, create it using helper.
            string UsersFile;
            try
            {
                UsersFile = System.IO.File.ReadAllText(SettingsController.SaveFileLocation + "Users.csv");
            }
            catch 
            {
                CreateUsersFile();
                UsersFile = System.IO.File.ReadAllText(SettingsController.SaveFileLocation + "Users.csv");
            }


        }

        //Private helper for establishing a Users.csv file, a file that will have a header row of the following: UID, First Name,
        //Last Name, (Specific InfoField1 from Settings), (Specific InfoField2), followed by rows where each row makes up a user
        private static void CreateUsersFile()
        {

        }
    }
}
