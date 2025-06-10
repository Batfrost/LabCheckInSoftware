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

        public LookupWindow()
        {
            InitializeComponent();

            //Get the SignUpWindow's specific custom info from the SettingsController
            string[] InfoFields = SettingsController.GetInfoFields();
            if (InfoFields[0] != null && InfoFields[0].Length > 0)
            {
                InfoField1Label.Content = InfoFields[0] + ": ";
                InfoField1Box.IsEnabled = true;
            }
            if (InfoFields[1] != null && InfoFields[1].Length > 0)
            {
                InfoField2Label.Content = InfoFields[1] + ": ";
                InfoField2Box.IsEnabled = true;
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
    }
}
