using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using LOLAutoSearching.Objects;
using System.Collections.ObjectModel;
using CapstoneDesign;
using LOLAutoSearching.ViewModel;
using System.Windows;
using System;
using LOLAutoSearching.Windows;

namespace LOLAutoSearching.Models
{
    public static class GameManager
    {
        public enum GameType {게임_선택 = 0, 비공개_선택 = 430, 랭크_게임 = 420, 무작위_총력전 = 920 };
        
        private static int gameWindowX = 0;
        private static int gameWindowY = 0;

        public static ObservableCollection<GameUser> Users { get; set; } = new ObservableCollection<GameUser>();

        private static ShowInformation mShowInformation;

        public static bool logixExit = false;

        public static async void LogixStart(ShowInformation showInformation)
        {
            showInformation.Hide();
            if (mShowInformation != null)
            {
                mShowInformation.ThisWindowClose = true;
                await Task.Delay(100);
                mShowInformation.Close();
            }
            mShowInformation = showInformation;
            Users.Clear();

            await CehckStartGame();

            if(FileControl.AutoSearching)
            {
                GameSearchLogicStart();
                await AddUsers();
            }
            else
            {
                if(gameType != GameType.무작위_총력전)
                {
                    BitmapSource capturedImage = ScreenCapture.GetGameType(gameWindowX, gameWindowY);
                    gameType = ImageSearchClass.GetGameType(capturedImage);
                }
                Debug.WriteLine("게임 타입 : " + gameType);
                ShowInformationViewModel.viewModelObject.GameType = gameType;
                await Task.Delay(2000);
                NewAddUsers();
            }
            GameLoop();
        }

        private static async Task CehckStartGame()
        {
            gameType = (GameType)(await ImageSearchClass.WaitCatchGame())[2];
            SetWindowPosition();
        }

        private static bool SetWindowPosition()
        {
            int[] result = ImageSearchClass.GameWindowPostion();
            gameWindowX = result[0];
            gameWindowY = result[1];
            ShowInformationViewModel.viewModelObject.GameWindowWidth = gameWindowX;
            ShowInformationViewModel.viewModelObject.GameWindowHeight = gameWindowY;
            return true;
        }

        private static async void GameLoop()
        {
            while (true)
            {
                if (GetProcessStatus.FineGameStartProcess())
                {
                    LogixStart(new ShowInformation());
                    return;
                }

                if (logixExit)
                {
                    logixExit = false;
                    LogixStart(new ShowInformation());
                    return;
                }

                if(ImageSearchClass.NewGame())
                {
                    LogixStart(new ShowInformation());
                    return;
                }

                SetWindowPosition();

                await Task.Delay(1000);
                if(gameWindowX == -1 && gameWindowY == -1)
                {
                    continue;
                }

                for (int index = 0; index < Users.Count; index++)
                {
                    GameUser user = Users[index];
                    if (user.data.accountID == string.Empty || user.data.userName == "이름 찾지 못함")
                    {
                        continue;
                    }

                    Champion usersChampion = MakeUserChampion(user);

                    if (usersChampion.code == 0)
                    {
                        user.champion = usersChampion;
                        user.championData = null;
                        ShowInformationViewModel.viewModelObject.UserChanged();
                        continue;
                    }

                    if (user.champion.code != usersChampion.code)
                    {
                        user.champion = usersChampion;
                        SetUserChampionData(user, gameType);
                    }
                }
            }
        }

        public static void UserMove(int FisrtUserIndex, int SecondUserIndex)
        {
            if(FisrtUserIndex >= 5
                || SecondUserIndex >= 5)
            {
                return;
            }

            if(FisrtUserIndex > Users.Count 
                || SecondUserIndex> Users.Count)
            {
                return;
            }

            var temp = Users[FisrtUserIndex];
            Users[FisrtUserIndex] = Users[SecondUserIndex];
            Users[SecondUserIndex] = temp;
            Users[FisrtUserIndex].index = FisrtUserIndex;
            Users[SecondUserIndex].index = SecondUserIndex;
        }

        public static GameType gameType = GameType.게임_선택;
        private static void GameSearchLogicStart()
        {
            BitmapSource capturedImage = ScreenCapture.GetGameType(gameWindowX, gameWindowY);
            string googleApiResult = SocketObject.GetImageSearching(LoginViewModel.LoginUser, ImageSearchClass.BufferFromImage(capturedImage));
            if (googleApiResult != null && googleApiResult != string.Empty)
            { googleApiResult = googleApiResult.Split('\n')[1].Split('\n')[0]; }
            if (googleApiResult == "비공개 선택")
            {
                gameType = GameType.비공개_선택;
            }
            else if (googleApiResult == "개인/2인 랭크 게임")
            {
                gameType = GameType.랭크_게임;
            }
            else if (googleApiResult == "무작위 총력전")
            {
                gameType = GameType.무작위_총력전;
            }
            else
            {
                //Debug.Assert(false);
            }
            Debug.WriteLine("게임 타입 : " + googleApiResult);
            return;
        }

        private static async void NewAddUsers()
        {
            InputControl.ChatContentCopy(gameWindowX + 40, gameWindowY + 650);

            await Task.Delay(1000); // 복붙 적용 시간 필요
            string[] userNames = Clipboard.GetText().Split(new string[]{ "\r\n" }, StringSplitOptions.None);

            for (int index = 0; index < userNames.Length; index++)
            {
                if(userNames[index].EndsWith("님이 로비에 참가하셨습니다."))
                {
                    userNames[index] = userNames[index].Split(new string[] { "님이 로비에 참가하셨습니다." }, StringSplitOptions.None)[0];
                }
                else if(userNames[index].EndsWith("님이 로비를 떠났습니다."))
                {
                    userNames[index] = userNames[index].Split(new string[] { "님이 로비를 떠났습니다." }, StringSplitOptions.None)[0];
                    foreach (GameUser deleteUser in Users)
                    {
                        if(deleteUser.data.userName == userNames[index])
                        {
                            Users.Remove(deleteUser);
                            break;
                        }
                    }
                    continue;
                }
                else
                {
                    continue;
                }

                if(userNames[index] == string.Empty)
                {
                    continue;
                }
                GameUser user = await CreateUserAsync(userNames[index]);
                user.index = index;
                Users.Add(user);
            }
        }

        private static async Task AddUsers()
        {
            BitmapSource capturedImage = await ScreenCapture.GetUsersAllImageAsync(gameWindowX, gameWindowY, gameType);
            string googleApiResult = SocketObject.GetImageSearching(LoginViewModel.LoginUser, ImageSearchClass.BufferFromImage(capturedImage));
            if (googleApiResult == null || googleApiResult == string.Empty)
            {
                Debug.Assert(false);
                return;
            }

            string[] googleAPIResuluts = googleApiResult.Split('\n');
            if (gameType == GameType.비공개_선택)
            {
                for (int index = 0; index < googleAPIResuluts.Length; index++)
                {
                    if (index % 2 == 0)
                    {
                        continue;
                    }
                    GameUser user = await CreateUserAsync(googleAPIResuluts[index]);
                    user.index = index / 2;
                    Users.Add(user);
                }
            }
            else if (gameType == GameType.무작위_총력전)
            {
                for (int index = 0; index < googleAPIResuluts.Length; index++)
                {
                    GameUser user = await CreateUserAsync(googleAPIResuluts[index]);
                    user.index = index;
                    Users.Add(user);
                }
            }
            else if (gameType == GameType.랭크_게임)
            {
                for (int index = 0; index < googleAPIResuluts.Length; index++)
                {
                    if (googleAPIResuluts[index] == "상단 (탑)"
                        || googleAPIResuluts[index] == "중단 (미드)"
                        || googleAPIResuluts[index] == "정글"
                        || googleAPIResuluts[index] == "하단 (봇)"
                        || googleAPIResuluts[index] == "서포터"
                        || googleAPIResuluts[index] == "희망 챔피언 선택"
                        || googleAPIResuluts[index] == string.Empty)
                    {
                        continue;
                    }
                    GameUser user = await CreateUserAsync(googleAPIResuluts[index]);
                    user.index = index / 2;
                    Users.Add(user);
                }
            }
        }

        private static async Task<GameUser> CreateUserAsync(string userName)
        {
            GameUser AddUser = new GameUser();
            if (userName == null)
            {
                AddUser.data = new GameUserData()
                {
                    accountID = string.Empty,
                    userName = "이름 인식 오류",
                };
                return AddUser;
            }

            AddUser.data = await Apis.GetUserData(userName);
            return AddUser;
        }

        private static Champion MakeUserChampion(GameUser user)
        {
            BitmapSource champCapturedImage;
            if (gameType == GameType.비공개_선택) {
                champCapturedImage = ScreenCapture.GetUserChampionOnNomal(gameWindowX, gameWindowY, user.index);
            }
            else if(gameType == GameType.랭크_게임)
            {
                //user.champion.nameKr = user.champion.nameKr + ImageSearchClass.UserIsSelect(gameWindowY, user.index);
                champCapturedImage = ScreenCapture.GetUserChampionOnRank(gameWindowX, gameWindowY, user);
                //MainWindow.image.Source = champCapturedImage;
            }
            else
            {
                champCapturedImage = ScreenCapture.GetUserChampionOnNomal(gameWindowX, gameWindowY, user.index);
            }
            return ChampionSearch.GetChapion(champCapturedImage);
        }
        
        private static async void SetUserChampionData(GameUser user, GameType gameType)
        {
            user.championData = await Apis.GetUserChampionData(user, gameType);
            ShowInformationViewModel.viewModelObject.UserChanged();
            while (user.championData.isUpdated == "false")
            {
                user.championData = await Apis.GetUserChampionDataToUpdate(user, gameType);
                ShowInformationViewModel.viewModelObject.UserChanged();
            }
            return;
        }
    }
}