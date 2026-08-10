using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    internal class OATokenStorage {
        private Dictionary<string, string> tokens = new Dictionary<string, string>();
        public OATokenStorage() {
            ReadStorageFile();
        }

        private void ReadStorageFile() {
            if (File.Exists(OAConstants.TOKEN_STORAGE_PATH)) {
                string[] lines = File.ReadAllLines(OAConstants.TOKEN_STORAGE_PATH);
                foreach (string line in lines) {
                    if (line.Length > 0) {
                        string[] parts = line.Split("=");
                        if (parts.Length == 2) {
                            tokens.Add(parts[0], parts[1]);
                        }
                    }
                }
            }
        }

        public void WriteStorageFile() {
            string output = "";

            foreach (string key in tokens.Keys) {
                output += key + "=" + tokens[key] + "\n";
            }

            Directory.CreateDirectory(OAConstants.TOKEN_STORAGE_FOLDER);
            File.WriteAllText(OAConstants.TOKEN_STORAGE_PATH, output);
        }

        public bool HasToken(string gamePublicId) {
            return tokens.ContainsKey(gamePublicId);
        }

        public string GetToken(string gamePublicId) {
            return HasToken(gamePublicId) ? tokens[gamePublicId] : "";
        }

        public void SetToken(string gamePublicId, string token) {
            if (HasToken(gamePublicId)) { 
                tokens[gamePublicId] = token;
            } else {
                tokens.Add(gamePublicId, token);
            }
        }
    }
}
