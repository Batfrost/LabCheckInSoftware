using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabCheckInSoftware
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

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
