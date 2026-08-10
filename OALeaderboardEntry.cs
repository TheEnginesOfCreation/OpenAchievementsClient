using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public class OALeaderboardEntry {

        public int Rank { get; private set; }
        public string Name { get; private set; }
        public int Score { get; private set; }
        public string Date { get; private set; }
        public bool IsCurrentUser { get; private set; }

        internal OALeaderboardEntry(int rank, string name, int score, string date, bool isCurrentUser) {
            Rank = rank;
            Name = name;
            Score = score;
            Date = date;
            IsCurrentUser = isCurrentUser;
        }
    }
}
