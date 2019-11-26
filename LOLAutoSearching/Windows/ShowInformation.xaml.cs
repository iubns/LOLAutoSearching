using LOLAutoSearching.Models;
using LOLAutoSearching.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LOLAutoSearching.Windows
{
    public partial class ShowInformation : Window
    {
        public static Image image;
        public bool ThisWindowClose { get; set; } = false;

        private ShowInformationViewModel viewModel;
        public ShowInformation()
        {
            InitializeComponent();
            Topmost = true;
            viewModel = new ShowInformationViewModel();
            DataContext = viewModel;
            viewModel.CloseAction = new Action(() => Close());
            SearchGameStart();
            Closed += (o,e) => { Close(); };
            image = test;
            if (LoginViewModel.LoginUser == null)
            {
                onLogin.Children.Clear();
            }
            else
            {
                notLogin.Children.Clear();
            }
        }
        
        private async void SearchGameStart()
        {
            while (ThisWindowClose == false)
            {
                if (viewModel.GameWindowHeight < 120 && viewModel.GameWindowWidth < 20)
                {
                    Hide();
                }
                else
                {
                    Show();

                }
                await Task.Delay(1000);
            }
        }

        private int moveListIndex = 0;
        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);
            int y = (int)point.Y - viewModel.GameWindowHeight - 60;
            GameManager.UserMove(y / 75, moveListIndex);
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);
            int y = (int)point.Y - viewModel.GameWindowHeight - 60;
            moveListIndex = y / 75;
        }

        private void ShowUserDetaileData(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);
            int y = (int)point.Y - viewModel.GameWindowHeight - 60;
            int userIndex = y / 75;
            viewModel.ShowUserDetaileData(userIndex);
        }
    }
}