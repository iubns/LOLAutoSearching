using System.Diagnostics;

namespace LOLAutoSearching.Models
{
    public static class GetProcessStatus
    {
        public static bool FineGameStartProcess()
        {
            return 1 <= Process.GetProcessesByName("League of Legends").Length;
        }

        public static bool FineGameOnProcess()
        {
            return 1 <= Process.GetProcessesByName("LeagueClientUx").Length;
        }
    }
}
