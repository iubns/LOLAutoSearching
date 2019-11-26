using LOLAutoSearching.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static LOLAutoSearching.Models.GameManager;

namespace LOLAutoSearching.Models
{
    public static class Apis
    {
        const string header_UA = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
        const string header_ConType = "application/x-www-form-urlencoded";
        const string serverDomain = "http://iubns.com/LOLAutoSearching";
        

        public static async Task<GameUserData> GetUserData(string userName)
        {
            do
            {
                string tempUserName = userName.Replace(" ", "%20");
                JObject getUserDataResult = await GetResult($"{ serverDomain }/GetGameUserData.php?userName={ tempUserName }");
                if (getUserDataResult["accountID"].ToString() != "UserFindError")
                {
                    return new GameUserData()
                    {
                        accountID = getUserDataResult["accountID"].ToString(),
                        userName = getUserDataResult["userName"].ToString(),
                        userLevel = getUserDataResult["level"].ToString(),
                        userTier = getUserDataResult["tier"].ToString(),
                    };
                }
                string[] tempSplut = userName.Split(' ');

                tempUserName = "";
                for (int index = 0; index < tempSplut.Length - 1; index++)
                {
                    if (index == tempSplut.Length - 2)
                    {
                        tempUserName += tempSplut[index];
                    }
                    else
                    {
                        tempUserName += tempSplut[index] + " ";
                    }
                }
                userName = tempUserName;
            } while (userName != "");
            return new GameUserData()
            {
                accountID = string.Empty,
                userName = "이름 찾지 못함",
            };
        }

        public static async Task<ChampionData> GetUserChampionData(GameUser userObject, GameType gameType)
        {
            JObject jObject =  await GetResult($"{serverDomain}/GetChampionDataByUser.php?accountID={userObject.data.accountID}&championCode={userObject.champion.code}&gameType={(int)gameType}");

            try
            {
                string kdaValue = string.Empty;
                if (jObject["deaths"].ToString() != "0")
                {
                    kdaValue = string.Format("{0:F2}", (float.Parse(jObject["kills"].ToString()) + float.Parse(jObject["assists"].ToString())) / float.Parse(jObject["deaths"].ToString()));
                }
                else
                {
                    kdaValue = "퍼팩트";
                }

                if (jObject["gameTime"].ToString() == "0")
                {
                    return new ChampionData()
                    {
                        kill = "0",
                        death = "0",
                        assist = "0",
                        gameTime = "0",
                        cs = "0",
                        winningRate = "0",
                        isUpdated = jObject["isUpdated"].ToString(),
                        kda = "0",
                    };
                }

                return new ChampionData()
                {
                    kill = jObject["kills"].ToString(),
                    death = jObject["deaths"].ToString(),
                    assist = jObject["assists"].ToString(),
                    gameTime = jObject["gameTime"].ToString(),
                    cs = jObject["cs"].ToString(),
                    winningRate = jObject["rate"].ToString(),
                    isUpdated = jObject["isUpdated"].ToString(),
                    kda = kdaValue,
                };
            }
            catch
            {
                Debug.Assert(false);
                return new ChampionData()
                {
                    isUpdated = "false"
                };
            }
        }

        public static async Task<ChampionData> GetUserChampionDataToUpdate(GameUser userObject, GameType gameType)
        {
            JObject jObject = await GetResult($"{serverDomain}/GetChampionDataByUserUpdate.php?accountID={userObject.data.accountID}&championCode={userObject.champion.code}&gameType={(int)gameType}");
            string kdaValue = string.Empty;
            if (jObject["deaths"].ToString() != "0")
            {
                kdaValue = string.Format("{0:F2}", (float.Parse(jObject["kills"].ToString()) + float.Parse(jObject["assists"].ToString())) / float.Parse(jObject["deaths"].ToString()));
            }
            else
            {
                kdaValue = "퍼팩트";
            }

            if(jObject["gameTime"].ToString() == "0")
            {
                return new ChampionData()
                {
                    kill = "0",
                    death = "0",
                    assist = "0",
                    gameTime = "0",
                    cs = "0",
                    winningRate = "0",
                    isUpdated = jObject["isUpdated"].ToString(),
                    kda = "0",
                };
            }

            ChampionData championData = new ChampionData()
            {
                kill = jObject["kills"].ToString(),
                death = jObject["deaths"].ToString(),
                assist = jObject["assists"].ToString(),
                gameTime = jObject["gameTime"].ToString(),
                cs = jObject["cs"].ToString(),
                winningRate = jObject["rate"].ToString(),
                isUpdated = jObject["isUpdated"].ToString(),
                kda = kdaValue,
            };
            return championData;
        }
        public static async Task<List<Game>> GetUserDetaileData(string UserAccountID, int ChampionCode, int GameType)
        {
            List<Game> games = new List<Game>();
            JArray jArray = await GetResultToArray($"{serverDomain}/GetUserDetaileData.php?accountID={UserAccountID}&championCode={ChampionCode}&gameType={GameType}");
            foreach (JToken jToken in jArray)
            {
                string kdaValue = string.Empty;

                if (jToken.Value<float>("deaths") != 0)
                {
                    kdaValue = string.Format("{0:F2}", (jToken.Value<float>("kills") + jToken.Value<float>("assists")) / jToken.Value<float>("deaths"));
                }
                else
                {
                    kdaValue = "퍼팩트";
                }

                DateTime gametime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(jToken.Value<double>("gameDuration"));
                DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(jToken.Value<double>("gameCreation"));
                string kpValue = string.Empty;
                if(jToken.Value<int>("teamKill") == 0)
                {
                    kpValue = "0%";
                }
                else
                {
                    kpValue = $"{((jToken.Value<int>("kills") + jToken.Value<int>("assists")) * 100 / jToken.Value<int>("teamKill")).ToString()}% ";
                }
                games.Add(new Game()
                {
                    gameHistoryUser = new GameHistoryUser()
                    {
                        win = $"/LOLAutoSearching;component/ResourceDictionarys/{jToken.Value<string>("win")}.png",
                        championData = new ChampionData()
                        {
                            kill = jToken.Value<string>("kills"),
                            death = jToken.Value<string>("deaths"),
                            assist = jToken.Value<string>("assists"),
                            cs = $"{jToken.Value<string>("cs")}({jToken.Value<string>("cspm")})",
                            kda = kdaValue,
                            kp = kpValue,
                        },
                        spellData = new SpellData()
                        {
                            spell1ID = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/spell/{SpellData.GetSpellName(jToken.Value<int>("sell1Id"))}.png",
                            spell2ID = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/spell/{SpellData.GetSpellName(jToken.Value<int>("sell2Id"))}.png",
                        }
                        ,
                        itemData = new ItemData()
                        {
                            itemImage0 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item0")}.png",
                            itemImage1 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item1")}.png",
                            itemImage2 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item2")}.png",
                            itemImage3 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item3")}.png",
                            itemImage4 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item4")}.png",
                            itemImage5 = $"http://ddragon.leagueoflegends.com/cdn/9.18.1/img/item/{jToken.Value<string>("item5")}.png",
                        },
                    },
                    date = $"{date.Year - 2000}/{date.Month}/{date.Day}",
                    gameTime = $"{gametime.Minute}m {gametime.Second}s",
                });
            }
            return games;
        }

        public static async Task<User> Login(string ID, string PW)
        {
            JObject json = await GetResult($"{serverDomain}/Login.php?ID={ID}&PW={PW}");
            if (json["Point"].ToString() == "-1")
            {
                return new User()
                {
                    Point = "-1",
                };
            }
            else
            {
                return new User()
                {
                    ID = ID,
                    PW = PW,
                    Point = json["Point"].ToString(),
                };
            }
        }

        public static async Task<User> LoginBySession(string Session)
        {
            JObject json = await GetResult($"{serverDomain}/LoginBySession.php?Session={Session}");
            if (json["Point"].ToString() == "-1")
            {
                return new User()
                {
                    Point = "-1",
                };
            }
            else
            {
                return new User()
                {
                    ID = json["ID"].ToString(),
                    Session = Session,
                    Point = json["Point"].ToString(),
                };
            }
        }

        public static async Task<string> MakeSession(string ID, string PW)
        {
            JObject json = await GetResult($"{serverDomain}/MakeSession.php?ID={ID}&PW={PW}");
            if (json["Sesstion"].ToString() == "0")
            {
                return string.Empty;
            }
            else
            {
                return json["Sesstion"].ToString();
            }
        }

        public static async Task<bool> CheckID(string ID)
        {
            JObject jObject = await GetResult($"{serverDomain}/CheckID.php?ID={ID}");
            if (jObject["ID"].ToString() == "0") // result is number of ID
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> Join(string ID, string PW)
        {
            JObject jObject = await GetResult($"{serverDomain}/Join.php?ID={ID}&PW={PW}");
            if (jObject["ID"].ToString() == "1") 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<JObject> GetNewVersion()
        {
            string url = $"{serverDomain}/GetNewVersion.php";
            return await GetResult(url);
        }

        public static void Update()
        {
            string browserUrl = $"http://iubns.iubns.net/LOLAutoSearchingInstaller.msi";
            WebClient webClient = new WebClient();
            webClient.DownloadFile(browserUrl, @"C:\ProgramData\Iubns\LOLAutoSearchingInstaller.msi");

            Process.Start(@"C:\ProgramData\Iubns\LOLAutoSearchingInstaller.msi");
            Process.GetCurrentProcess().Kill();
        }

        private static async Task<JObject> GetResult(string url)
        {
            HttpWebRequest web = (HttpWebRequest)WebRequest.Create(url);
            web.UserAgent = header_UA;
            web.ContentType = header_ConType;
            using (var streamReaderResult = new StreamReader((await web.GetResponseAsync()).GetResponseStream()))
            {
                try
                {
                    string returl = streamReaderResult.ReadToEnd();
                    Debug.WriteLine($"json url = {url} \n결과 : {returl}");
                    return JObject.Parse(returl);
                }
                catch
                {
                    Debug.Assert(false);
                    return null;
                }
            }
        }
        private static async Task<JArray> GetResultToArray(string url)
        {
            HttpWebRequest web = (HttpWebRequest)WebRequest.Create(url);
            web.UserAgent = header_UA;
            web.ContentType = header_ConType;
            using (var streamReaderResult = new StreamReader((await web.GetResponseAsync()).GetResponseStream()))
            {
                try
                {
                    string returl = streamReaderResult.ReadToEnd();
                    Debug.WriteLine($"json url = {url} \n결과 : {returl}");
                    return JArray.Parse(returl);
                }
                catch
                {
                    Debug.Assert(false);
                    return null;
                }
            }
        }
    }
}
