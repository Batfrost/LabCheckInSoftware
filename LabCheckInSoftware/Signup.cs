
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

            //Check if user already exists or not, and if they do, just return
            if (Lookup.LookupUser(uID, null, null)[0] is not null)
                return;

            //Change the uID to a standard form, as sometimes users enter '0' as the first char or 'u'
            uID = 'u' + uID.Substring(1);

            //Add the user's info and the date they registered as a new row into Users.csv
            UsersFile += "\n" + uID + "," + fName + "," + lName + "," + infoField1 + "," + infoField2 + "," + DateTime.Now.ToString("MM/dd/yyyy");
            System.IO.File.WriteAllText(SettingsController.SaveFileLocation + "Users.csv", UsersFile);
        }

        //Private helper for establishing a Users.csv file, a file that will have a header row of the following: UID, First Name,
        //Last Name, (Specific InfoField1 from Settings), (Specific InfoField2), followed by rows where each row makes up a user
        private static void CreateUsersFile()
        {
            string[] InfoFields = SettingsController.GetInfoFields();
            string UsersFile = "uID,First Name,Last Name," + InfoFields[0] + "," + InfoFields[1] + ",Date Registered";
            System.IO.File.WriteAllText(SettingsController.SaveFileLocation + "Users.csv", UsersFile);
        }
    }
}
