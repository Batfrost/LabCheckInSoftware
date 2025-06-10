using System.Windows;
using System.Windows.Controls;

namespace TWLogging
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            string[] infoFields = SettingsController.GetInfoFields();
            string UserAgreementText = SettingsController.GetUserAgreementText();
            if (infoFields[0] != null && infoFields[0].Length > 0)
                InfoField1TextBox.Text = infoFields[0];
            if (infoFields[1] != null && infoFields[1].Length > 0)
                InfoField2TextBox.Text = infoFields[1];
            SignupInfoTextBox.Text = UserAgreementText;

        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
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

            //Check if a password has been entered, and confirm both password boxes match
            if (!NewPasswordBox.Password.Equals(ConfirmPasswordBox.Password))
                popupText.Text += "New Passwords do not match.\n";

            if (NewPasswordBox.Password.Length > 0 || OldPasswordBox.Password.Length > 0)
                if (SettingsController.CheckPassword(OldPasswordBox.Password))
                    popupText.Text += "Old Password is incorrect.\n";

            //Check if the User Agreement text has been filled out
            if (SignupInfoTextBox.Text.Length == 0)
                popupText.Text += "Please fill out a User Agreement for the Sign up page to use.\n";

            //If something wasn't filled out correctly, make the popup with one button for closing itself when acknowledged
            if (popupText.Text.Length > 0)
            {
                Viewbox v1 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
                //v1.Child = closeButton;
                popupGrid.Children.Add(closeButton);
                Grid.SetRow(closeButton, 1);
                warningPopup.ShowDialog();
                return; //Don't establish settings if something isn't correct
            }

            //Establish settings
            SettingsController.EstablishSettings(NewPasswordBox.Password, SignupInfoTextBox.Text, InfoField1TextBox.Text, InfoField2TextBox.Text);
            //Doubly make sure everything is disconnected and shut down
            if (closeButton != null) closeButton.Click -= PopupCloseButton_Click;
            if (warningPopup != null) warningPopup.Close();
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
