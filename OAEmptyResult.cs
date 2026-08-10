using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public class OAEmptyResult {
        public OAResultStatus Status { get; private set; }

        public OAEmptyResult(int oaResponseStatus) {
            switch (oaResponseStatus) {
                case OAResponse.STATUS_OK:
                    Status = OAResultStatus.Ok;
                    break;
                case OAResponse.STATUS_SESSION_EXPIRED:
                    Status = OAResultStatus.NoValidSession;
                    break;
                case OAResponse.STATUS_HTTP_ERROR:
                    Status = OAResultStatus.HttpError;
                    break;
                case OAResponse.STATUS_USER_DOES_NOT_EXIST:
                    Status = OAResultStatus.UserDoesNotExist;
                    break;
                default:
                    Status = OAResultStatus.InternalError;
                    break;
            }
        }

        public OAEmptyResult(OAResultStatus status) { 
            Status = status;
        }
    }
}
