using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
        private ObservableCollection<CheckedInRow> CheckedInDataGridRows = new ObservableCollection<CheckedInRow>();

        public HomeWindow()
        {
            loginTimer = new DispatcherTimer();
            InitializeComponent();

            //Setup the DataGrid that will display who is currently checked in to the software.
            string[] infoFields = SettingsController.GetInfoFields();

            ////Only display the info field columns that are not empty strings
            //if (!infoFields[0].Equals(""))
            //    CheckedInDataGrid.Columns.Add(new DataGridTextColumn { Header = infoFields[0] });

            //if (!infoFields[1].Equals(""))
            //    CheckedInDataGrid.Columns.Add(new DataGridTextColumn { Header = infoFields[1] });

            ////Last column will be time they checked in
            //CheckedInDataGrid.Columns.Add(new DataGridTextColumn() { Header = "Time Checked In" });
            CheckedInDataGrid.ItemsSource = CheckedInDataGridRows;

        }

        //Event Handler for attempting to check in a user when 8 characters are entered into the CheckInBox (8 because that's the size of a uID)
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

                //Update the CheckedInGrid - If this user already is on the datagrid, remove them and relay that they checked out, otherwise add and relay checked in
                bool checkingOut = false;
                foreach (var row in CheckedInDataGridRows)
                {
                    if (row.uID!.Equals(text))
                    {
                        CheckedInDataGridRows.Remove(row);
                        WhoLoggedInLabel.Content = usersName + " checked out at " + DateTime.Now.ToShortTimeString();
                        checkingOut = true;
                        break;
                    }
                }
                if (!checkingOut)
                {
                    string[] usersInfo = Lookup.LookupUser(text, null, null);
                    CheckedInDataGridRows.Add(new CheckedInRow() { uID = usersInfo[0], fName = usersInfo[1], lName = usersInfo[2], infoField1 = usersInfo[3], infoField2 = usersInfo[4], time = DateTime.Now.ToShortTimeString() });
                    WhoLoggedInLabel.Content = usersName + " checked in at " + DateTime.Now.ToShortTimeString();
                }

                //Automatically hide the text that states who just checked in after a minute
                timeCounter = 0;
                loginTimer.Tick += LoginTimer_Tick!;
                loginTimer.Interval = TimeSpan.FromSeconds(1);
                loginTimer.Start();
            }
        }

        //Timer handler for automatically hiding the text that states a user just checked in after a minute
        private void LoginTimer_Tick(object sender, EventArgs e)
        {
            timeCounter++;
            if (timeCounter == 60)
            {
                loginTimer.Stop();
                WhoLoggedInLabel.Content = "";
            }
        }

        //Event Handler for creating the columns/headers for the CheckedInDataGrid: Sets the column headers to be correct text, based on the DisplayName property of a field in the Row class below
        private void GenerateColumnsAndHeaders(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dispName = GrabDisplayName(e.PropertyDescriptor);

            //If the displayName is null, then the actual name of the field will be used instead
            if (dispName is not null)
            {
                string[] infoFields = SettingsController.GetInfoFields();
                if (dispName.Equals("infF1"))
                {
                    e.Column.Header = infoFields[0];
                    if (infoFields[0].Equals(""))
                        e.Column.Visibility = Visibility.Collapsed;
                }
                else if (dispName.Equals("infF2"))
                {
                    e.Column.Header = infoFields[1];
                    if (infoFields[1].Equals(""))
                        e.Column.Visibility = Visibility.Collapsed;
                }
                else e.Column.Header = dispName;
            }
        }

        //Private helper for getting the DisplayName property text from a field with the property in the Row class below
        private string? GrabDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                //Check for the attribute and return the DisplayName
                var dispName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (dispName != null && dispName != DisplayNameAttribute.Default)
                {
                    return dispName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    //Check for attribute and return the DisplayName
                    Object[] attr = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attr.Length; ++i)
                    {
                        var dispName = attr[i] as DisplayNameAttribute;
                        if (dispName != null && dispName != DisplayNameAttribute.Default)
                        {
                            return dispName.DisplayName;
                        }
                    }
                }
            }
            //Just return null if there isn't any property for this field
            return null;
        }
    }

    /// <summary>
    /// This class contains the data for one Row that will make up the CheckedInDataGrid
    /// </summary>
    public class CheckedInRow()
    {
        public string? uID {  get; set; }
        [DisplayName("First Name")]
        public string? fName { get; set; }
        [DisplayName("Last Name")]
        public string? lName { get; set; }
        [DisplayName("infF1")]
        public string? infoField1 { get; set; }
        [DisplayName("infF2")]
        public string? infoField2 { get; set; }
        [DisplayName("Time Checked In")]
        public string? time { get; set; }
    }
}