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

namespace Dispatch
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtUserName.Text))
                return;
            if (string.IsNullOrEmpty(TxtPassword.Password))
                return;

            BtnSignIn.IsEnabled = false;

            try
            {
                Logger.Log.Info("Logging in with username: " + TxtUserName.Text);

                await Shared._Parser.LoginAsync(TxtUserName.Text, TxtPassword.Password);

                Shared.UserName = TxtUserName.Text.Trim();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error when sign in");
                Logger.Log.Error(ex);
                MessageBox.Show(ex.Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            BtnSignIn.IsEnabled = true;
        }
    }
}
