using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private DispatcherTimer CloseWindowTimer;
        private int TimeCounter = 0;
        //Lock for when both mouse movement and tick events change TimeCounter value
        private static readonly object Lock = new object();

        public ManagerWindow()
        {
            InitializeComponent();

            //Start a timer that resets everytime the mouse is moved, and when it reaches 30 seconds, closes this window.
            CloseWindowTimer = new DispatcherTimer();
            CloseWindowTimer.Tick += CloseWindowTimer_Tick!;
            CloseWindowTimer.Interval = TimeSpan.FromSeconds(1);
            CloseWindowTimer.Start();
        }

        //Takes user to Settings Window
        private void ChangeSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //Initialize the Settings Window to match this windows size settings
            var SW = new SettingsWindow()
            {
                WindowStartupLocation = this.WindowStartupLocation,
                Width = this.Width,
                Height = this.Height,
                WindowState = this.WindowState,
                WindowStyle = this.WindowStyle
            };
            SW.Show();
            this.Close();
        }

        //Takes user to AttendanceTracker Window
        private void AttendanceTrackerButton_Click(object sender, RoutedEventArgs e)
        {
            //Initialize the AttendanceTracker Window to match this windows size settings
            var ATW = new AttendanceTrackerWindow()
            {
                WindowStartupLocation = this.WindowStartupLocation,
                Width = this.Width,
                Height = this.Height,
                WindowState = this.WindowState,
                WindowStyle = this.WindowStyle
            };
            ATW.Show();
            this.Close();
        }

        //Takes user to Lookup Window
        private void LookupButton_Click(object sender, RoutedEventArgs e)
        {
            //Initialize the Lookup Window to match this windows size settings
            var LW = new LookupWindow()
            {
                WindowStartupLocation = this.WindowStartupLocation,
                Width = this.Width,
                Height = this.Height,
                WindowState = this.WindowState,
                WindowStyle = this.WindowStyle
            };
            LW.Show();
            this.Close();
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

        //Opens the save file in a new file explorer window
        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\explorer.exe";
            psi.Arguments = SettingsController.SaveFileLocation;
            Process.Start(psi);
        }
    }
}
