using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using shittyEtsy.Data.Models;
using shittyEtsy.Data;
using BCrypt;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace shittyEtsy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        

        private void CreateAnAccount_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string name = UsernameInput.Text;
            string password = PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Please enter username and password";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            using var db = new AppDataContext();

            var user = db.Users.FirstOrDefault(u => u.Name == name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ErrorText.Text = "Invalid username or password";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            else
            {
                Frame.Navigate(typeof(HomePage));
            }
        }
    }
}
