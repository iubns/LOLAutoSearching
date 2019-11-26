using LOLAutoSearching.Models;
using LOLAutoSearching.Objects;
using LOLAutoSearching.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace LOLAutoSearching.ViewModel
{
    public class LoginViewModel : SuperViewModel
    {
        public static LoginViewModel viewModelObject;
        public LoginViewModel()
        {
            viewModelObject = this;
        }

        public string ID { get; set; } = string.Empty;
        public string PW { get; set; } = string.Empty;
        public static User LoginUser { get; set; }
        public bool AutoLogin { get; set; } = false;

        public Action CloseAction { get; set; }

        private ICommand _Login;
        public ICommand Login => _Login ?? (_Login = new CommandHandler(async () => {
            User user = await Apis.Login(ID, PW);
            if (user.Point == "-1")
            {
                MessageBox.Show("아이디 또는 비밀번호 확인");
                return;
            }

            if(AutoLogin)
            {
                FileControl.Session = await Apis.MakeSession(ID, PW);
            }

            LoginUser = user;
            GameManager.logixExit = true;
            CloseAction();
        }));

        private ICommand _Join;
        public ICommand Join => _Join ?? (_Join = new CommandHandler(() =>
        {
            new Join().Show();
        }));
    }
}