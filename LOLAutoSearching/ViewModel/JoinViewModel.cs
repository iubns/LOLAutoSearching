using LOLAutoSearching.Models;
using System.Windows;
using System.Windows.Input;

namespace LOLAutoSearching.ViewModel
{
    public class JoinViewModel : SuperViewModel
    {
        public string ID { get; set; }
        bool IDis = false;

        private ICommand _IDCheck;
        public ICommand IDCheck => _IDCheck ?? (_IDCheck = new CommandHandler(async () =>
        {
            IDis = await Apis.CheckID(ID);
            if (IDis)
            {
                MessageBox.Show("사용 가능한 ID");
            }
            else
            {
                MessageBox.Show("이미 사용중인 아이디 입니다.\n다시 ID를 설정해 주세요.");
                ID = string.Empty;
                OnProperChanged("ID");
            }
        }));
    }
}
