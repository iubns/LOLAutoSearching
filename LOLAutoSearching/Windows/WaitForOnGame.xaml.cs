using LOLAutoSearching.Models;
using LOLAutoSearching.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LOLAutoSearching.Windows
{
    public partial class WaitForOnGame : Window
    {
        public WaitForOnGame()
        {
            VersionCheck();
            InitializeComponent();
            SessionLogin();
        }

        private async void SessionLogin()
        {
            if(FileControl.Session == string.Empty)
            {
                FileControl.AutoSearching = false;
            }
            else
            {
                LoginViewModel.LoginUser = await Apis.LoginBySession(FileControl.Session);
            }
        }

        private async void VersionCheck()
        {
            if(File.Exists(@"‪C:\Temp\LOLAutoSearchingInstaller.msi"))
            {
                File.Delete(@"‪C:\Temp\LOLAutoSearchingInstaller.msi");
            }
            DirectoryInfo directory = new DirectoryInfo(@"C:\ProgramData\Iubns");
            if (directory.Exists == false)
            {
                directory.Create();
            }

            JObject result = await Apis.GetNewVersion();
            if (result["NewVersion"].ToString() != "1.0.0.26")
            {
                MessageBox.Show($"\"{result["PatchContent"].ToString() }\"이(가) 적용된 새로운 버전{ result["NewVersion"].ToString() }이 있습니다.\n업데이트를 진행 합니다.", "LOLSearch");
                Apis.Update();
            }
        }

        private async void WaitForGame(object o, EventArgs e)
        {
            Directory.SetCurrentDirectory(Process.GetCurrentProcess().MainModule.FileName.Split(new string[] { "\\LOLAutoSearching.exe" }, StringSplitOptions.None)[0]);
            Hide();
            while (true)
            {
                if(GetProcessStatus.FineGameOnProcess())
                {
                    GameManager.LogixStart(new ShowInformation());
                    Close();
                    return;
                }
                await Task.Delay(1000 * 10);
            }
        }
    }
}
