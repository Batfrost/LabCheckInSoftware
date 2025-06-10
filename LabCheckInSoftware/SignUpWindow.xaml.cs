using System.Windows;
using System.Windows.Controls;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();

            //Get the SignUpWindow's specific custom info from the SettingsController
            UserAgreementText.Text = SettingsController.GetUserAgreementText();
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
        }

        //Handler for checking and taking in all inputted information for a new user, and if the info is acceptable, adds a new user to the system
        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            //Check and make sure all required info field boxes are filled out
            //Establish the new window that will serve as the popup for telling user if something still needs to be filled out or not
            Window warningPopup = new Window() { Width = 500, Height = 200 };
            Grid popupGrid = new Grid();
            warningPopup.Content = popupGrid;
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            popupGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Viewbox v0 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
            TextBlock popupText = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center }; popupText.Text = "";
            v0.Child = popupText;
            popupGrid.Children.Add(v0);
            Grid.SetRow(v0, 0);
            Button closeButton = new Button() { Content = "Okay" };
            closeButton.Click += PopupCloseButton_Click;

            if (uIDBox.Text.Length < 8)
                popupText.Text += "UID needs to be 8 characters, can start with either '0' or 'u'.\n";
            if (FirstNameBox.Text.Length == 0)
                popupText.Text += "Please enter a first name.\n";
            if (LastNameBox.Text.Length == 0)
                popupText.Text += "Please enter a last name.\n";
            if ((InfoField1Box.IsEnabled == true && InfoField1Box.Text.Length == 0) || (InfoField2Box.IsEnabled == true && InfoField2Box.Text.Length == 0))
                popupText.Text += "Please fill out all available textboxes.\n";

            //If something wasn't filled out correctly, make the popup with one button for closing itself when acknowledged
            if (popupText.Text.Length > 0)
            {
                Viewbox v1 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
                //v1.Child = closeButton;
                popupGrid.Children.Add(closeButton);
                Grid.SetRow(closeButton, 1);
                warningPopup.ShowDialog();
                return; //Don't add this user if something isn't correct
            }

            Signup.AddNewUser(uIDBox.Text, FirstNameBox.Text, LastNameBox.Text, InfoField1Box.Text, InfoField2Box.Text);
            var HW = new HomeWindow
            {
                WindowStartupLocation = this.WindowStartupLocation,
                Width = this.Width,
                Height = this.Height,
                WindowState = this.WindowState,
                WindowStyle = this.WindowStyle
            };
            HW.Show();
            //Doubly make sure everything is disconnected and shut down
            if (closeButton != null) closeButton.Click -= PopupCloseButton_Click;
            if (warningPopup != null) warningPopup.Close();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var HW = new HomeWindow
            {
                WindowStartupLocation = this.WindowStartupLocation,
                Width = this.Width,
                Height = this.Height,
                WindowState = this.WindowState,
                WindowStyle = this.WindowStyle
            };
            HW.Show();
            this.Close();
        }

        //Private handler for the new popup window's button to close the window 
        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button!.Click -= PopupCloseButton_Click;
            var win = Window.GetWindow(button);
            win.Close();
        }
    }
}
