using System.Threading.Channels;
using System.Windows;
using System.Windows.Threading;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for LookupWindow.xaml
    /// </summary>
    public partial class LookupWindow : Window
    {
        private DispatcherTimer CloseWindowTimer;
        private int TimeCounter = 0;
        //Lock for when both mouse movement and tick events change TimeCounter value
        private static readonly object Lock = new object();

        private bool InfoField1Enabled = false;
        private bool InfoField2Enabled = false;
        private string[]? LoadedUser;

        public LookupWindow()
        {
            InitializeComponent();

            //Get the LookupWindow's specific custom info from the SettingsController for the custom info fields
            string[] InfoFields = SettingsController.GetInfoFields();
            if (InfoFields[0] != null && InfoFields[0].Length > 0)
            {
                InfoField1Label.Content = InfoFields[0] + ": ";
                InfoField1Box.IsEnabled = true;
                InfoField1Enabled = true;
            }
            if (InfoFields[1] != null && InfoFields[1].Length > 0)
            {
                InfoField2Label.Content = InfoFields[1] + ": ";
                InfoField2Box.IsEnabled = true;
                InfoField2Enabled = true;
            }

            //Start a timer that resets everytime the mouse is moved, and when it reaches 30 seconds, closes this window.
            CloseWindowTimer = new DispatcherTimer();
            CloseWindowTimer.Tick += CloseWindowTimer_Tick!;
            CloseWindowTimer.Interval = TimeSpan.FromSeconds(1);
            CloseWindowTimer.Start();
        }

        //Tick event for when the CloseWindowTimer will close the window
        private void CloseWindowTimer_Tick(object sender, EventArgs e)
        {
            lock (Lock)
            {
                TimeCounter++;
                TimerLabel.Content = (30 - TimeCounter).ToString();
                if (TimeCounter == 30)
                {
                    CloseWindowTimer.Stop();
                    this.Close();
                }
            }
        }

        //Everytime the window detects mouse movement, reset the close window timer
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            lock (Lock)
            {
                TimeCounter = 0;
            }
        }

        //When 'Enter' is pressed on this Name Lookup box, attempts to perform a lookup based on Name
        private void NameLookupBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                try
                {
                    string[] name = NameLookupBox.Text.Split(' ');
                    string[] info = Lookup.LookupUser(null, name[0], name[1]);
                    if (info[0] is not null)  //If the lookup is successful, the first string shouldn't be empty
                        LookupFinishedSuccessfully(info);
                    else
                    {
                        //Lookup failed, notify user on GUI
                        LookupSuccessLabel.Content = "Lookup Failed at " + DateTime.Now.ToShortTimeString();
                    }
                }
                catch
                { //Lookup failed, notify user on GUI
                    LookupSuccessLabel.Content = "Lookup Failed at " + DateTime.Now.ToShortTimeString();
                }
            }
        }

        //When 'Enter' is pressed on this UID Lookup box, attempts to perform a lookup based on UID
        private void UIDLookupBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string[] info = Lookup.LookupUser(UIDLookupBox.Text, null, null);
                if (info[0] is not null) //If the lookup is successful, the first string shouldn't be empty
                    LookupFinishedSuccessfully(info);
                else
                {
                    //Lookup failed, notify user on GUI
                    LookupSuccessLabel.Content = "Lookup Failed at " + DateTime.Now.ToShortTimeString();
                }
            }
        }

        //Private helper for when a lookup is successful, puts all info from lookup into GUI
        private void LookupFinishedSuccessfully(string[] userInfo)
        {
            //Lookup is successful, let's notify user on GUI
            LookupSuccessLabel.Content = "Lookup Successful";

            //Fill the Info Textboxes with the info that has been looked up
            uIDBox.Text = userInfo[0];
            FirstNameBox.Text = userInfo[1];
            LastNameBox.Text = userInfo[2];
            if (InfoField1Enabled)
                InfoField1Box.Text = userInfo[3];
            if (InfoField2Enabled)
                InfoField2Box.Text = userInfo[4];
            LoadedUser = userInfo;
        }

        //Changes the currently loaded user's info - uID
        private void uIDBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && LoadedUser is not null)
            {
                Lookup.EditUserInfoGivenID(LoadedUser[0], uIDBox.Text, null, null, null, null);
            }
        }

        //Changes the currently loaded user's info - First Name
        private void FirstNameBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && LoadedUser is not null)
            {
                Lookup.EditUserInfoGivenID(LoadedUser[0], null, FirstNameBox.Text, null, null, null);
            }
        }

        //Changes the currently loaded user's info - Last Name
        private void LastNameBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && LoadedUser is not null)
            {
                Lookup.EditUserInfoGivenID(LoadedUser[0], null, null, LastNameBox.Text, null, null);
            }
        }

        //Changes the currently loaded user's info - Custom Info Field 1
        private void InfoField1Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && LoadedUser is not null)
            {
                Lookup.EditUserInfoGivenID(LoadedUser[0], null, null, null, InfoField1Box.Text, null);
            }
        }

        //Changes the currently loaded user's info - Custom Info Field 2
        private void InfoField2Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && LoadedUser is not null)
            {
                Lookup.EditUserInfoGivenID(LoadedUser[0], null, null, null, null, InfoField2Box.Text);
            }
        }
    }
}
