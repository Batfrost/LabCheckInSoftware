using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private DispatcherTimer CheckinTimer;
        private int timeCounter = 0;
        private DispatcherTimer CheckoutTimer;
        private ObservableCollection<CheckedInRow> CheckedInDataGridRows = new ObservableCollection<CheckedInRow>();
        private DispatcherTimer MarkAbsencesTimer;

        public HomeWindow()
        {
            CheckinTimer = new DispatcherTimer();
            InitializeComponent();

            //Setup the DataGrid that will display who is currently checked in to the software.
            string[] infoFields = SettingsController.GetInfoFields();
            CheckedInDataGrid.ItemsSource = CheckedInDataGridRows;

            CheckoutTimer = new DispatcherTimer();
            CheckoutTimer.Tick += CheckoutTimer_Tick!;
            CheckoutTimer.Interval = TimeSpan.FromMinutes(1);
            CheckoutTimer.Start();

            //Mark attendances with Tracker method every day, doesn't mark absence for current day
            MarkAbsencesTimer = new DispatcherTimer();
            MarkAbsencesTimer.Tick += MarkAbsencesTimer_Tick!;
            MarkAbsencesTimer.Interval = TimeSpan.FromDays(1);
            MarkAbsencesTimer.Start();

        }

        //Event Handler for attempting to enter the manager mode, asks user to enter a password, and if correct, takes user to manager page
        private void ManagerMode_Clicked(object sender, EventArgs e)
        {
            //First ask for the Manager Password on a new window
            Window ManagerPasswordWindow = new Window() { Width = 500, Height = 200 };
            Grid popupGrid = new Grid();
            ManagerPasswordWindow.Content = popupGrid;
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Viewbox v0 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
            TextBlock popupText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, Text = "Please enter the Manager Password. \nIf forgotten, ask Trevor."};
            v0.Child = popupText;
            popupGrid.Children.Add(v0);
            Grid.SetRow(v0, 0);
            PasswordBox pB = new PasswordBox() { Name = "pB" };
            popupGrid.Children.Add(pB);
            Grid.SetRow(pB, 1);
            Button closeButton = new Button() { Content = "Log In" };
            closeButton.Click += ManagerModePasswordWindow;
            popupGrid.Children.Add(closeButton);
            Grid.SetRow(closeButton, 2);
            ManagerPasswordWindow.ShowDialog();

            while (ManagerPasswordWindow!.IsActive)
            {
                //Doubly make sure everything is disconnected and shut down
                if (closeButton != null) closeButton.Click -= ManagerModePasswordWindow;
                if (ManagerPasswordWindow != null) ManagerPasswordWindow.Close();
            }
        }

        //Private handler for the new popup window's button to check the inputted password and close if correct
        private void ManagerModePasswordWindow(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var win = Window.GetWindow(button);
            Grid? grid = win.Content as Grid;
            PasswordBox? pB = (PasswordBox?)grid!.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == 1 && Grid.GetColumn(e) == 0);

            //Check the Inputted Password
            if (SettingsController.CheckPassword(pB!.Password))
            {
                //Initialize the ManagerWindow to match this windows size settings
                var MW = new ManagerWindow
                {
                    WindowStartupLocation = this.WindowStartupLocation,
                    Width = this.Width,
                    Height = this.Height,
                    WindowState = this.WindowState,
                    WindowStyle = this.WindowStyle
                };
                MW.Show();
                button!.Click -= ManagerModePasswordWindow;
                win.Close();
            }
            else
            {
                TextBlock? incorrectPasswordText = (TextBlock?)grid!.Children.Cast<UIElement>().FirstOrDefault(e => Grid.GetRow(e) == 3 && Grid.GetColumn(e) == 0);
                if (incorrectPasswordText is null)
                {
                    incorrectPasswordText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, Text = "Password entered at " + DateTime.Now.ToShortTimeString() + " is incorrect." };
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.Children.Add(incorrectPasswordText);
                    Grid.SetRow(incorrectPasswordText, 3);
                }
                else
                {
                    incorrectPasswordText.Text = "Password entered at " + DateTime.Now.ToString("h:mm:ss tt") + " is incorrect.";
                }
            }
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
                    if (row.uID!.Equals('u' + text.Substring(1)))
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

                //Check their attendance
                Tracker.CheckAttendanceForUser(text);

                //Automatically hide the text that states who just checked in after a minute
                timeCounter = 0;
                CheckinTimer.Tick += CheckinTimer_Tick!;
                CheckinTimer.Interval = TimeSpan.FromSeconds(1);
                CheckinTimer.Start();
            }
        }

        //Timer handler for automatically hiding the text that states a user just checked in after a minute
        private void CheckinTimer_Tick(object sender, EventArgs e)
        {
            timeCounter++;
            if (timeCounter == 60)
            {
                CheckinTimer.Stop();
                WhoLoggedInLabel.Content = "";
            }
        }

        //Timer handler for automatically checking a user out by simply removing them from the CheckedInDataGrid
        private void CheckoutTimer_Tick(object sender, EventArgs e)
        {
            foreach (var row in CheckedInDataGridRows)
            {
                if (row.time is not null && DateTime.Parse(row.time).AddMinutes(1) <= DateTime.Now)
                {
                    CheckedInDataGridRows.Remove(row);
                    break;
                }
            }
        }

        //Timer handler for automatically checking for absences for the previous days
        private void MarkAbsencesTimer_Tick(object sender, EventArgs e)
        {
            Tracker.MarkAbsences();
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