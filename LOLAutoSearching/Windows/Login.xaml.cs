using LOLAutoSearching.Models;
using LOLAutoSearching.Objects;
using LOLAutoSearching.ViewModel;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace LOLAutoSearching.Windows
{
    public partial class Login : Window
    {
        LoginViewModel loginViewModel;
        public Login()
        {
            InitializeComponent();
            loginViewModel = new LoginViewModel();
            DataContext = loginViewModel;
            loginViewModel.CloseAction = new Action(() => Close());
        }

        private async void PW_PasswordChanged(object sender, RoutedEventArgs e)
        {
            User user = await Apis.Login(loginViewModel.ID, PW.Password);
            if (user.Point != "-1")
            {
                if (loginViewModel.AutoLogin)
                {
                    FileControl.Session = await Apis.MakeSession(loginViewModel.ID, PW.Password); ;
                }

                LoginViewModel.LoginUser = user;
                GameManager.logixExit = true;
                Close();
            }
        }
    }
}
