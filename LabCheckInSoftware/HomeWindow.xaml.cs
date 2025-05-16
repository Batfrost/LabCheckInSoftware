using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private DispatcherTimer loginTimer;
        private int timeCounter = 0;

        public HomeWindow()
        {
            loginTimer = new DispatcherTimer();
            InitializeComponent();
        }

        private void CheckInBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = CheckInTextBox.Text;
            if (text.Length == 8) //uIDs are 8 characters long
            {
                //First attempt to check in this user, if method fails/returns null, take user to SignUpWindow
                string? usersName = CheckIn.AttemptCheckIn(text);
                if (usersName is null)
                {
                    //Initialize the SignUpWindow to match this windows size settings
                    var SUW = new SignUpWindow
                    {
                        WindowStartupLocation = this.WindowStartupLocation,
                        Width = this.Width,
                        Height = this.Height,
                        WindowState = this.WindowState,
                        WindowStyle = this.WindowStyle
                    };
                    SUW.Show();
                    this.Close();
                }
                CheckInTextBox.Text = "";
                WhoLoggedInLabel.Content = usersName + " logged in at " + DateTime.Now.ToShortTimeString();
                //Automatically hide the text that states who just logged in after a minute
                timeCounter = 0;
                loginTimer.Tick += LoginTimer_Tick!;
                loginTimer.Interval = TimeSpan.FromSeconds(1);
                loginTimer.Start();
            }
        }

        private void LoginTimer_Tick(object sender, EventArgs e)
        {
            timeCounter++;
            if (timeCounter == 60)
            {
                loginTimer.Stop();
                WhoLoggedInLabel.Content = "";
            }
        }
    }
}