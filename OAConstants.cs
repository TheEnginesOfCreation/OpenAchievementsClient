using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    internal class OAConstants {
        public static string VERSION = "1.1.0";
        public static string SERVICE_URL = "https://www.open-achievements.com/service/service.php";
        public static string TOKEN_STORAGE_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TheEnginesOfCreation\\OpenAchievements\\";
        public static string TOKEN_STORAGE_PATH = TOKEN_STORAGE_FOLDER + "oa.dat";

        public static string ENDPOINT_USER = "user";
        public static string ENDPOINT_ACHIEVEMENTS = "achievements";
        public static string ENDPOINT_LEADERBOARDS = "leaderboards";
    }
}
