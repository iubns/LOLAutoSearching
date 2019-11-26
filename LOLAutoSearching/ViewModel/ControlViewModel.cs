using LOLAutoSearching.Models;
using LOLAutoSearching.Windows;
using System;

namespace LOLAutoSearching.ViewModel
{
    public class ControlViewModel : SuperViewModel
    {
        public ControlViewModel()
        {
            ID = LoginViewModel.LoginUser.ID;
            Point = LoginViewModel.LoginUser.Point;
            OnProperChanged("ID");
            OnProperChanged("Point");
            GameManager.LogixStart(new ShowInformation());
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
        
        public Array GameTypeArray { get; set; } = Enum.GetValues(typeof(GameManager.GameType));

        private GameManager.GameType _GameTypeValue = GameManager.gameType;
        public GameManager.GameType GameTypeValue
        {
            get
            {
                return _GameTypeValue;
            }
            set
            {
                _GameTypeValue = value;
                GameManager.gameType = value;
            }
        }
         
        public Action CloseAction { get; set; }
    }
}