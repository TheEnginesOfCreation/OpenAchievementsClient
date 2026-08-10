using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public class OALeaderboard : IEnumerable {

        private List<OALeaderboardEntry> entries = new List<OALeaderboardEntry>();

        public string Name { get; private set; }
        public int EntryCount { get; private set; }

        internal OALeaderboard(string name, int entryCount) {
            Name = name;
            EntryCount = entryCount;
        }

        internal void AddEntry(OALeaderboardEntry entry) {
            entries.Add(entry);
        }

        public IEnumerator GetEnumerator() {
            return entries.GetEnumerator();
        }
    }
}
