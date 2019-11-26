using LOLAutoSearching.Models;
using System.Windows;

namespace LOLAutoSearching.Windows
{
    public partial class Join : Window
    {
        bool IDis = false;
        bool PWis = false;

        public Join()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void CheckIDAsync(object sender, RoutedEventArgs e)
        {
            IDis = await Apis.CheckID(ID.Text);
            if (IDis)
            {
                MessageBox.Show("사용 가능한 ID");
            }
            else
            {
                MessageBox.Show("이미 사용중인 아이디 입니다.\n다시 ID를 설정해 주세요.");
                ID.Text = string.Empty;
            }
        }

        private void PWCheckChanged(object sender, RoutedEventArgs e)
        {
            if(PW.Password != PWCheck.Password)
            {
                PWCheckLabel.Content = "비밀번호가 다릅니다.";
                PWis = false;
            }
            else
            {
                PWCheckLabel.Content = "비밀번호가 같습니다.";
                PWis = true;
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void RegistAsync(object sender, RoutedEventArgs e)
        {
            if(IDis == false)
            {
                MessageBox.Show("중복확인을 해주세요.");
                return;
            }

            if(PWis == false)
            {
                MessageBox.Show("비밀번호가 서로 다릅니다.");
                return;
            }

            await Apis.Join(ID.Text, PW.Password);
            Close();
        }
    }
}
