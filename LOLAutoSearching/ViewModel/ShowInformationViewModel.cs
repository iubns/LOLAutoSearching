using LOLAutoSearching.Models;
using LOLAutoSearching.Objects;
using LOLAutoSearching.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using static LOLAutoSearching.Models.GameManager;

namespace LOLAutoSearching.ViewModel
{
    public class ShowInformationViewModel : SuperViewModel
    {
        public Action CloseAction { get; set; }

        public static ShowInformationViewModel viewModelObject;
        public ShowInformationViewModel()
        {
            viewModelObject = this;
            if (LoginViewModel.LoginUser != null)
            {
                ID = LoginViewModel.LoginUser.ID;
                Point = LoginViewModel.LoginUser.Point;
                OnProperChanged("ID");
                OnProperChanged("Point");
            }
        }

        public ObservableCollection<GameUser> Users { get; set; } = GameManager.Users;
        public ObservableCollection<Game> UsersGames { get; set; } = new ObservableCollection<Game>();

        public void UserChanged()
        {
            CollectionViewSource.GetDefaultView(Users).Refresh();
            OnProperChanged("users");
        }

        private int _gameWindowWidth;
        public int GameWindowWidth
        {
            get {return _gameWindowWidth; }
            set
            {
                _gameWindowWidth = value += 15;
                OnProperChanged("gameWindowWidth");
            }
        }

        private int _gameWindowHeight;
        public int GameWindowHeight
        {
            get { return _gameWindowHeight; }
            set
            {
                _gameWindowHeight = value + 105 - 60;
                OnProperChanged("gameWindowHeight");
            }
        }

        public bool AutoSearching
        {
            get
            {
                return FileControl.AutoSearching;
            }
            set
            {
                FileControl.AutoSearching = value;
                OnProperChanged("AutoSearching");
            }
        }

        private string _ID { get; set; }
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                OnProperChanged("ID");
            }
        }

        private string _Point { get; set; }
        public string Point
        {
            get
            {
                return _Point;
            }
            set
            {
                _Point = value;
                OnProperChanged("Point");
            }
        }

        public GameType _GameType { get; set; }
        public GameType GameType
        {
            get
            {
                return _GameType;
            }
            set
            {
                _GameType = value;
                OnProperChanged("GameType");
            }
        }

        private ICommand _Login;
        public ICommand Login => _Login ?? (_Login = new CommandHandler(() =>
        {
            new Login().Show();
        }));

        public GameUser _DetaileDataDameUser { get; set; }
        public GameUser DetaileDataDameUser
        {
            get
            {
                return _DetaileDataDameUser;
            }
            set
            {
                _DetaileDataDameUser = value;
                OnProperChanged("DetaileDataDameUser");
            }
        }


        public async void ShowUserDetaileData(int userIndex)
        {
            UsersGames.Clear();
            DetaileDataDameUser = Users[userIndex];
            List<Game> games = await Apis.GetUserDetaileData(DetaileDataDameUser.data.accountID, DetaileDataDameUser.champion.code, (int)gameType);
            foreach(Game game in games)
            {
                UsersGames.Add(game);
            }
        }
    }
}