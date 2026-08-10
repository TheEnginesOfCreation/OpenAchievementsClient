using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public enum OAResultStatus {
        Ok,
        NoValidSession,
        InternalError,
        UserDoesNotExist,
        GameDoesNotExist,
        HttpError
    }
}
