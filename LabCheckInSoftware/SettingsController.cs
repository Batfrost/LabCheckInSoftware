
namespace TWLogging
{
    /// <summary>
    /// This class will control the Settings file for the software, which will manage:
    ///     -the password (not great practice, I know),
    ///     -the information for the Sign Up page
    /// </summary>
    internal static class SettingsController
    {
        public static string SaveFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\TWLogging\\";

        /// <summary>
        /// Will create the Settings file based on the info given from the SetupWindow
        /// </summary>
        public static void EstablishSettings(string password, string userAgreementText, string? infoField1, string? infoField2)
        {
            if (!System.IO.Directory.Exists(SaveFileLocation))
                System.IO.Directory.CreateDirectory(SaveFileLocation.Substring(0, SaveFileLocation.LastIndexOf('\\')));
            string settings = "P: " + password + "\nUAT: " + userAgreementText + "\nIF1: " + infoField1 + "\nIF2: " + infoField2;
            System.IO.File.WriteAllText(SaveFileLocation + "Settings.config", EncryptText(settings));
        }

        /// <summary>
        /// Checks if settings are established or not, for the purpose of skipping the Setup stuff if already setup.
        /// Also makes sure that the settings contain all necessary info.
        /// </summary>
        public static bool CheckForSettings()
        {
            try
            {
                string settings = DecryptText(System.IO.File.ReadAllText(SaveFileLocation + "Settings.config"));
                if (settings != null && settings.Contains("P: ") && settings.Contains("UAT: ") && settings.Contains("IF1: ") && settings.Contains("IF2: "))
                    return true;
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Replaces the password in the settings file with the given new password.
        /// Again, this is very bad software practice for actually dealing with a password,
        /// but for the purposes of this software, great password safety isn't very necessary
        /// </summary>
        public static void ChangePassword(string newPassword)
        {
            string settings = DecryptText(System.IO.File.ReadAllText(SaveFileLocation + "Settings.config"));
            settings = "P: " + newPassword + settings.Substring(settings.IndexOf('\n'));
            System.IO.File.WriteAllText(SaveFileLocation + "Settings.config", EncryptText(settings));
        }

        /// <summary>
        /// Compares the given password (pToCheck) with the actual stored password
        /// </summary>
        public static bool CheckPassword(string pToCheck)
        {
            string settings = DecryptText(System.IO.File.ReadAllText(SaveFileLocation + "Settings.config"));
            string password = settings.Substring(3, settings.IndexOf("\n") - 3);

            return pToCheck.Equals(password);
        }

        /// <summary>
        /// Finds and returns the custom info fields from settings
        /// </summary>
        public static string[] GetInfoFields()
        {
            string[] infoFields = new string[2];

            string settings = DecryptText(System.IO.File.ReadAllText(SaveFileLocation + "Settings.config"));
            infoFields[0] = settings.Substring(settings.IndexOf("IF1: ") + 5).Split('\n')[0];
            infoFields[1] = settings.Substring(settings.IndexOf("IF2: ") + 5);

            return infoFields;
        }

        /// <summary>
        /// Finds and returns the text that makes up the user agreement from settings
        /// </summary>
        public static string GetUserAgreementText()
        {
            string settings = DecryptText(System.IO.File.ReadAllText(SaveFileLocation + "Settings.config"));
            int firstIndex = settings.IndexOf("UAT: ") + 5;
            int lastIndex = settings.IndexOf("IF1: ") - firstIndex;
            return settings.Substring(firstIndex, lastIndex);
        }

        /// <summary>
        /// Encrypts given text to be unreadable to anyone who doesn't know how to decrypt.
        /// Adds just a little bit of security to the system, so that the settings files aren't messed with.
        /// </summary>
        private static string EncryptText(String text)
        {
            Random r = new Random();
            char[] chars = new char[10] { 'a', 't', 'k', 'l', 'w', 'q', 'p', 'P', 'A', 'T' };
            string encryptedText = "";
            int evenFactor = (text.Length % 2 == 0) ? 0 : 1;
            for (int i = text.Length - 1; i >= 1; i -= 2)
            {
                encryptedText += text[i - (1 + evenFactor)];
                encryptedText += text[i - evenFactor];
                encryptedText += chars[r.Next(9)];
            }
            
            return encryptedText;
        }

        /// <summary>
        /// Decrypts the text given that was specifically encrypted with the Encryption method above.
        /// </summary>
        private static string DecryptText(String text)
        {
            string decryptedText = "";
            while (text.Length > 0)
            {
                decryptedText += text[text.Length - 3];
                decryptedText += text[text.Length - 2];
                text = text.Substring(0, text.Length - 3);
            }

            return decryptedText;
        }
    }
}
