using System.Collections.Generic;

namespace Mahjong
{
    public class Stats
    {
        private List<Game> _games;
        
        public List<Game> Games
        {
            get { return _games; }
        }

        public Stats()
        {
            _games = new List<Game>();
        }
    }
}