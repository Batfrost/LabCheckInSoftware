using System.Formats.Asn1;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for AttendanceTrackerWindow.xaml
    /// </summary>
    public partial class AttendanceTrackerWindow : Window
    {
        public AttendanceTrackerWindow()
        {
            InitializeComponent();
        }

        //Button Handler that makes sure all necessary info is filled out correctly, and then creates an Attendance Tracker using Tracker.cs
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            //Establish the new window that will serve as the popup for telling user if something still needs to be filled out or not
            Window warningPopup = new Window() { Width = 600, Height = 300 };
            Grid popupGrid = new Grid();
            warningPopup.Content = popupGrid;
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Viewbox v0 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
            TextBlock popupText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center }; popupText.Text = "";
            v0.Child = popupText;
            popupGrid.Children.Add(v0);
            Grid.SetRow(v0, 1);
            try
            {
                if (!FromDatePicker.SelectedDate.HasValue || !ToDatePicker.SelectedDate.HasValue)
                    popupText.Text += "Please fill out a From and To Date.\n";
                else if (FromDatePicker.SelectedDate > ToDatePicker.SelectedDate)
                    popupText.Text += "The From Date needs to be before the To Date.\n";

                if (!((bool)MondayCheckBox.IsChecked! || (bool)TuesdayCheckBox.IsChecked! || (bool)WednesdayCheckBox.IsChecked! || (bool)ThursdayCheckBox.IsChecked! || (bool)FridayCheckBox.IsChecked!))
                    popupText.Text += "Please select at least one day of the week.\n";

                if (NameTextBox.Text is null || NameTextBox.Text.Length == 0)
                    popupText.Text += "Please input a Name for the tracker.\n";

                if (IDsToTrackTextBox.Text is null || IDsToTrackTextBox.Text.Length == 0)
                    popupText.Text += "Please input at least one uID to track.\n";
            }
            catch (Exception ex)
            {
                popupText.Text += "Something went wrong, here's the error message:\n" + ex.Message;
            }

            //Attempt to split the IDsToTrack Text box into individual IDs
            string allIDsTogether = IDsToTrackTextBox.Text!.Replace("\r", string.Empty).Replace(" ", "").Trim(' ').Replace("\n", string.Empty).Trim('\n');
            List<string> IDsToTrack = new List<string>();
            if (allIDsTogether.Length % 8 != 0)
                popupText.Text += "Combined count of ID's chars aren't divisible by 8, some may have been inputted wrong.\n";
            else
                for (int i = 0; i < allIDsTogether.Length; i += 8)
                    IDsToTrack.Add(allIDsTogether.Substring(i, 8));

            //If something wasn't filled out correctly, make the popup with one button for closing itself when acknowledged
            if (popupText.Text.Length > 0)
            {
                warningPopup.ShowDialog();
                return; //Don't establish settings if something isn't correct
            }
            bool[] validDaysOfWeek = [(bool)MondayCheckBox.IsChecked!, (bool)TuesdayCheckBox.IsChecked!,(bool)WednesdayCheckBox.IsChecked!, (bool)ThursdayCheckBox.IsChecked!, (bool)FridayCheckBox.IsChecked!];
            bool success = Tracker.CreateAttendanceTracker(NameTextBox.Text!, (DateTime)FromDatePicker.SelectedDate!, (DateTime)ToDatePicker.SelectedDate!, validDaysOfWeek, (bool)BiweeklyCheckBox.IsChecked!, IDsToTrack.ToArray());

            
            Window successPopup = new Window() { Width = 600, Height = 300 };
            Grid successGrid = new Grid();
            successPopup.Content = successGrid;
            successGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            successGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            successGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            successGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Viewbox v2 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
            TextBlock successText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center }; successText.Text = (success) ? "Attendance Tracker created Successfully." : "Failed to Create Attendance Tracker, please try again.";
            v2.Child = successText;
            successGrid.Children.Add(v2);
            Grid.SetRow(v2, 1);
            if (success)
            {
                successPopup.ShowDialog();
                //Doubly make sure everything is disconnected and shut down
                if (warningPopup != null) warningPopup.Close();
                this.Close();
            }
        }
    }
}
