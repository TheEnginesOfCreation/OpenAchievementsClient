# OpenAchievementsClient
C# implementation of OpenAchievements Web API client. For more information about OpenAchievements, visit https://www.open-achievements.com

## Getting started

...

## Usage

### Instantiate a new client and performing requests

```csharp
OAClient oaClient = new OAClient("game_id", "game_private_id");
```
The game's ID and private ID can be found on the game's developer page. Make sure that your game's private ID will **always stay private** and is not shared with others.
The resulting client instance can be used to make requests to the OpenAchievements backend. Each of these requests will return an `OAResult<>` object. The generic
datatype depends on the type of request that was made. The requests will be done asynchronously through C# `async`/`await`.

---

**class OAResult**

*properties*
* `Status` (`OAResultStatus`) The resulting status for this request. See `OAResultStatus` enum for more information.
* `Result` The response data for this request.

---

**enum OAResultStatus**

* `Ok` The request was handled correctly and the response is valid.
* `NoValidSession` The request requires a valid session but no valid session token was supplied. Create a new session to continue using the client.
* `UserDoesNotExist` The user for which the request is made does not exist.
* `GameDoesNotExist` The game for which the request is made does not exist.
* `InternalError` An other unspecified error has occurred.
* `HttpError` A HTTP error has occured.

---

### Check session status

Use the `GetSessionStatus()` method on your client to check the status of the current session. 

```csharp
OAResult<OASessionStatus> result = await oaClient.GetSessionStatus();
```

The resulting `OAResult` contains an `OASessionStatus` value.

---
**enum OASessionStatus**

* `Active` There is an active session and further requests can be made to OpenAchievements.
* `NoSession` There is no valid session. A new session will need to be created.
* `AwaitingConfirmation` There is a session but it has not been approved by the user yet.
---

### Create a new session

To create a new session for a user you can use the `CreateSession()` method. This method takes the user's OpenAchievements ID or their email address as argument. Both are strings, both are valid.
The resulting boolean is `true` when the session was successfully created and `false` if not.

```csharp
OAResult<bool> result = await oaClient.CreateSession("Somebody#8573");
```

### Get currently connected user's displayname

The string only contains the user's displayname, not the numeric part of their OpenAchievements ID (the four-digit number after the #). The user's email address
will also not be returned, even if they supplied their email address to create a new session.

```csharp
OAResult<string> result = await oaClient.GetCurrentUser();
```

### Check if user has unlocked an achievement

The private ID for the achievement can be found on the game's developer page. Note that you should **never share private achievement IDs with anyone**.

```csharp
OAResult<bool> result = await oaClient.IsAchievementUnlocked("achievement_private_id");
```

### Get achievements that user has currently unlocked

The `GetUnlockedAchievements()` method's result contains a list of private ID's for achievements unlocked by the current user.

```csharp
OAResult<List<string>> result = await oaClient.GetUnlockedAchievements();
```

### Unlock a single achievement for the user

The private ID for the achievement can be found on the game's developer page. Note that you should **never share private achievement IDs with anyone**. The resulting boolean is `true` when the achievement was succesfully unlocked and `false` if not. If the achievement was already unlocked, this will result in `false` being returned.

```csharp
OAResult<bool> result = await oaClient.UnlockAchievement("achievement_private_id");
```

### Unlock multiple achievements at once for the user

The private ID for achievements can be found on the game's developer page. Note that you should **never share private achievement IDs with anyone**. The resulting boolean is `true` when all of the achievements were succesfully unlocked and `false` if not. If any of the achievements were already unlocked, this will result in `false` being returned.

```csharp
List<string> privateIds = new List<String> { "achievement_private_id_1", "achievement_private_id_2", "achievement_private_id_3" };
OAResult<bool> result = await oaClient.UnlockAchievements(privateIds);
```

### Get leaderboard data

The private ID for the leaderboard can be found on the game's developer page. Note that you should **never share private leaderboard IDs with anyone**.
<br>The `offset` argument is an `int` value that determines how many entries are skipped in the resulting data.
<br>The `limit` argument is an `int` value that determines the maximum number of entries that are returned. The maximum limit is 50. Any number higher than 50 will be capped at 50.
<br>If you want to get data for positions 11 up to (and including) 25 of the leaderboard, your `offset` would be 10 and your `limit` would be 15.
<br>The resulting data is returned as an `OALeaderboard` object.

```csharp
int offset = 0;
int limit = 25;
OAResult<OALeaderboard> result = await oaClient.GetLeaderboardData("leaderboard_private_id", offset, limit);
```

---

**class OALeaderboard**

*properties*
* `Name` (`string`) The name of the leaderboard.
* `EntryCount` (`int`) The total number of entries on this leaderboard.

*methods*
* `GetEnumerator()` (`IEnumerator`) Gets an enumerator to enumerate over the leaderboard entries. Each entry is represented as a `OALeaderboardEntry` object.

Because `OALeaderboard` implements `IEnumerable`, the entries can be enumerated through with a for-each loop

```csharp
OALeaderboard oaLeaderboard = result.Result;
foreach (OALeaderboardEntry entry in oaLeaderboard) {
    ...
}
```
---
**class OALeaderboardEntry**

*properties*
* `Rank` (`int`) The rank of this entry on the leaderboard.
* `Name` (`string`) The displayname of the user whose entry this is on the leaderboard.
* `Score` (`int`) The score for this entry.
* `Date` (`string`) The date at which this entry was created or last updated. The date is a `string` formatted as YYYY-MM-dd.
* `IsCurrentUser` (`bool`) Boolean indicating whether or not this is the current user's entry on this leaderboard.
---

### Get leaderboard score for current user

The private ID for the leaderboard can be found on the game's developer page. Note that you should **never share private leaderboard IDs with anyone**.
If the current user doesn't have an entry on this leaderboard yet, the resulting score will be 0.

```csharp
OAResult<int> result = await oaClient.GetLeaderboardScore("leaderboard_private_id");
```

### Submit a leaderboard score for current user

The private ID for the leaderboard can be found on the game's developer page. Note that you should **never share private leaderboard IDs with anyone**.
The resulting boolean will be `true` when the score was successfully set on the leaderboard and `false` if not.

```csharp
int score = 2175;
OAResult<bool> result = await oaClient.SubmitLeaderboardScore("leaderboard_private_id", score);
```