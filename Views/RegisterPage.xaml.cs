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
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace shittyEtsy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameInput.Text;
            string email = EmailInput.Text;
            string password = PasswordInput.Password;
            string confirmPassword = ConfirmPasswordInput.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword ) || string.IsNullOrWhiteSpace(email))
            {
                ErrorText.Text = "Please fill in all fields.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (password != confirmPassword)
            {
                ErrorText.Text = "Passwords do not match.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            using var db = new AppDataContext();
            if (db.Users.Any(u => u.Name == name))
            {
                ErrorText.Text = "Username already exists.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new Data.Models.User
            {
                Name = name,
                Email = email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,

            };

            db.Users.Add(newUser);
            db.SaveChanges();
            Frame.Navigate(typeof(LoginPage));
        }



        private void GoToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
    }
}
