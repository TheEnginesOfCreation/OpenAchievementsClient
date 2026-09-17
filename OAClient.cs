using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public class OAClient {
        private string gamePublicId;
        private string gamePrivateId;

        private OATokenStorage tokenStorage;

        
        public bool DebugEnabled { get; set; }

        public OAClient(string gamePublicId, string gamePrivateId) {
            this.gamePublicId = gamePublicId;
            this.gamePrivateId = gamePrivateId;

            tokenStorage = new OATokenStorage();
        }

        public string GetVersion() {
            return OAConstants.VERSION;
        }

        /// <summary>Checks the status of the client's session.</summary>
        /// <returns>
        ///     Any value of <c>OASessionStatus</c>.
        ///     Active means there is an active session and requests can be done.
        ///     NoSession means the client has no session.
        ///     AwaitingConfirmation means there is a session but the user has not authorized it yet.
        /// </returns>
        public async Task<OAResult<OASessionStatus>> GetSessionStatus() {
            if (!tokenStorage.HasToken(gamePublicId)) {
                return new OAResult<OASessionStatus>(OAResultStatus.Ok, OASessionStatus.NoSession);
            }

            OARequest request = CreateRequest(OAConstants.ENDPOINT_USER, "getgamesessionstatus");

            OAResponse response = await request.DoRequest();
            
            if (response.Status == OAResponse.STATUS_OK) {
                string strSessionStatus = response.GetJson().GetString("sessionStatus");
                Enum.TryParse(strSessionStatus, out OASessionStatus sessionStatus);

                return new OAResult<OASessionStatus>(OAResultStatus.Ok, sessionStatus);
            } else {
                return new OAResult<OASessionStatus>(OAResultStatus.InternalError, OASessionStatus.NoSession);
            }
        }

        /// <summary>Creates a new session for a user.</summary>
        /// <param name="id">Either the email address the user used to register at OpenAchievements or the DisplayName#idTag ID.</param>
        /// <param name="description">
        /// The description for this connection request. Will be visible to user. 
        /// Recommended to supply some information here that the user can use to identify the device from which the request originated, like device name.
        /// </param>
        /// <returns>True when a new session was created, false when no new session was created.</returns>
        public async Task<OAResult<bool>> CreateSession(string id, string description) {
            OAResult<OASessionStatus> result = await GetSessionStatus();

            if (result.Result != OASessionStatus.NoSession) {
                return new OAResult<bool>(false);
            }

            OARequest request = CreateRequest(OAConstants.ENDPOINT_USER, "creategamesession");
            request.AddArgument("id", id);
            request.AddArgument("description", description);

            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                tokenStorage.SetToken(gamePublicId, response.GetJson().GetString("token"));
                tokenStorage.WriteStorageFile();
                return new OAResult<bool>(true);
            } else if (response.Status == OAResponse.STATUS_USER_DOES_NOT_EXIST) {
                return new OAResult<bool>(OAResultStatus.UserDoesNotExist, false);
            } else if (response.Status == OAResponse.STATUS_GAME_DOES_NOT_EXIST) {
                return new OAResult<bool>(OAResultStatus.GameDoesNotExist, false);
            }
            
            return new OAResult<bool>(OAResultStatus.InternalError, false);
        }

        /// <summary>Unlocks an achievement for the current user.</summary>
        /// <param name="achievementPrivateId">The private ID of the achievement to unlock for the user.</param>
        /// <returns>True if the achievement was unlocked, false if not (it could be the achievement was already unlocked).</returns>
        public async Task<OAResult<bool>> UnlockAchievement(string achievementPrivateId) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_ACHIEVEMENTS, "unlock");
            request.AddArgument("ids", achievementPrivateId);
            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<bool>(response.GetJson().GetBool("unlocked"));
            } else {
                return new OAResult<bool>(response.Status, false);
            }
        }

        /// <summary>Unlocks multiple achievements for the current user.</summary>
        /// <param name="achievementPrivateIds">The private IDs of the achievements to unlock for the user.</param>
        /// <returns>True if all of the achievements were unlocked, false if not all were unlocked (it could be there were already unlocked achievements).</returns>
        public async Task<OAResult<bool>> UnlockAchievements(List<string> achievementPrivateIds) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_ACHIEVEMENTS, "unlock");
            request.AddArgument("ids", achievementPrivateIds);
            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<bool>(response.GetJson().GetBool("unlocked"));
            } else {
                return new OAResult<bool>(response.Status, false);
            }
        }

        /// <summary>Checks if an achievement is unlocked for the current user</summary>
        /// <param name="achievementPrivateId">The private ID of the achievement to check</param>
        /// <returns>true if the achievement is unlocked, false if not</returns>
        public async Task<OAResult<bool>> IsAchievementUnlocked(string achievementPrivateId) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_ACHIEVEMENTS, "isunlocked");
            request.AddArgument("id", achievementPrivateId);
            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<bool>(response.GetJson().GetBool("unlocked"));
            } else {
                return new OAResult<bool>(response.Status, false);
            }
        }

        /// <summary>Gets a list of private IDs for achievements the user has unlocked</summary>
        /// <returns></returns>
        public async Task<OAResult<List<string>>> GetUnlockedAchievements() {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_ACHIEVEMENTS, "getunlocked");
            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                List<string> list = new List<string>();

                return new OAResult<List<string>>(OAResultStatus.Ok, response.GetJson().GetList<string>("achievements"));
            } else {
                return new OAResult<List<string>>(OAResultStatus.InternalError, new List<string>());
            }
        }

        /// <summary>Gets the displayname for the current user</summary>
        /// <returns></returns>
        public async Task<OAResult<string>> GetCurrentUser() {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_USER, "getcurrentuser");
            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<string>(response.GetJson().GetString("name"));
            } else {
                return new OAResult<string>(response.Status, "");
            }
        }

        /// <summary>Submit a new highscore to a leaderboard for the current user</summary>
        /// <param name="leaderboardPrivateId">Private ID of the leaderboard to submit score for</param>
        /// <param name="score">New highscore for the user</param>
        /// <returns></returns>
        public async Task<OAResult<bool>> SubmitLeaderboardScore(string leaderboardPrivateId, int score) {
            return await SubmitLeaderboardScore(leaderboardPrivateId, score, null);
        }

        /// <summary>Submit a new highscore to a leaderboard for the current user</summary>
        /// <param name="leaderboardPrivateId">Private ID of the leaderboard to submit score for</param>
        /// <param name="score">New highscore for the user</param>
        /// <param name="verificationData">Developer supplied data (max 16.384 characters) the developer can use to verify the high score claim</param>
        /// <returns></returns>
        public async Task<OAResult<bool>> SubmitLeaderboardScore(string leaderboardPrivateId, int score, string? verificationData) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_LEADERBOARDS, "submitscore");
            request.AddArgument("leaderboardPrivateId", leaderboardPrivateId);
            request.AddArgument("score", score);
            if (verificationData != null) {
                request.AddArgument("verificationData", verificationData);
            }

            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<bool>(true);
            } else {
                return new OAResult<bool>(response.Status, false);
            }
        }

        /// <summary>Retrieve leaderboard highscore for the current user</summary>
        /// <param name="leaderboardPrivateId">Private ID of the leaderboard to retrieve user's score for</param>
        /// <returns>The user's highscore on the specified leaderboard</returns>
        public async Task<OAResult<int>> GetLeaderboardScore(string leaderboardPrivateId) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_LEADERBOARDS, "getuserscore");
            request.AddArgument("leaderboardPrivateId", leaderboardPrivateId);

            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                return new OAResult<int>(response.GetJson().GetInt("score"));
            } else {
                return new OAResult<int>(response.Status, 0);
            }
        }

        /// <summary>Retrieve leaderboard and leaderboard highscore data for a leaderboard</summary>
        /// <param name="leaderboardPrivateId">Private ID of the leaderboard to retrieve data for</param>
        /// <param name="offset">The number of highscore entries to skip (ordered from highest score to lowest score)</param>
        /// <param name="limit">The number of highscore entries to retrieve</param>
        /// <returns></returns>
        public async Task<OAResult<OALeaderboard>> GetLeaderboardData(string leaderboardPrivateId, int offset, int limit) {
            OARequest request = CreateRequest(OAConstants.ENDPOINT_LEADERBOARDS, "getdata");
            request.AddArgument("leaderboardPrivateId", leaderboardPrivateId);
            request.AddArgument("offset", offset);
            request.AddArgument("limit", limit);

            OAResponse response = await request.DoRequest();

            if (response.Status == OAResponse.STATUS_OK) {
                OAJson leaderboardJson = response.GetJson().GetJson("leaderboard");
                OALeaderboard leaderboard = new OALeaderboard(leaderboardJson.GetString("name"), leaderboardJson.GetInt("entryCount"));

                List<OAJson> list = response.GetJson().GetJsonArray("entries");
                foreach (OAJson entryJson in list) {
                    OALeaderboardEntry entry = new OALeaderboardEntry(
                        entryJson.GetInt("rank"),
                        entryJson.GetString("displayName"),
                        entryJson.GetInt("score"),
                        entryJson.GetString("date"),
                        entryJson.GetBool("currentUser")
                    );
                    leaderboard.AddEntry(entry);
                }

                return new OAResult<OALeaderboard>(leaderboard);
            } else {
                return new OAResult<OALeaderboard>(response.Status, null);
            }
        }

        private OARequest CreateRequest(string endpoint, string method) {
            return new OARequest(endpoint, method, DebugEnabled, gamePrivateId, tokenStorage.GetToken(gamePublicId));
        }
    }
}
