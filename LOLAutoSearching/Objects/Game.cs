using System.Collections.Generic;

namespace LOLAutoSearching.Objects
{
    public class Game
    {
        public GameHistoryUser gameHistoryUser { get; set; }
        public string date { get; set; }
        public string gameTime { get; set; }
        public List<GameHistoryUser> attendUsers { get; set; }
    }
}
