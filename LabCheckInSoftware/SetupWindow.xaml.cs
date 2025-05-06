using System.Windows;
using System.Windows.Controls;

namespace LabCheckInSoftware
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        Window warningPopup = new Window();

        public SetupWindow()
        {
            InitializeComponent();
        }

        //Doubly makes sure that when the settings window is closed, any child windows (the popup window in this case from the ConfirmButton_Click) are also closed.
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        /// <summary>
        /// When a user presses this button, this method checks if all necessary inputs are filled out,
        /// and if so, then establishes new settings for software, if not so, gives popup warning for user
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //Establish the new window that will serve as the popup for telling user if something still needs to be filled out or not
            warningPopup = new Window() { Width = 500, Height = 200 };
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
            
            //Check if a password has been entered, and confirm both password boxes match
            if (PasswordBox.Password.Length == 0)
                popupText.Text += "Please create a password.\n";
            if (!PasswordBox.Password.Equals(ConfirmPasswordBox.Password))
                popupText.Text += "Passwords do not match.\n";

            //Check if the User Agreement text has been filled out
            if (SignupInfoTextBox.Text.Length == 0)
                popupText.Text += "Please fill out a User Agreement for the Sign up page to use.\n";

            //If something wasn't filled out correctly, make the popup with one button for closing itself when acknowledged
            if (popupText.Text.Length > 0)
            {
                Viewbox v1 = new Viewbox() { HorizontalAlignment = HorizontalAlignment.Center, Stretch = System.Windows.Media.Stretch.Uniform };
                Button closeButton = new Button() { Content = "Okay" };
                closeButton.Click += PopupCloseButton_Click;
                //v1.Child = closeButton;
                popupGrid.Children.Add(closeButton);
                Grid.SetRow(closeButton, 1);
                warningPopup.ShowDialog();
            }
            
            //Establish settings

        }

        //Private handler for the new popup window's button to close the window 
        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            warningPopup.Close();
        }

    }
}
