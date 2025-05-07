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

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
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
            }
        }
    }
}