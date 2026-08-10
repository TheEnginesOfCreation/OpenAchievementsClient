using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    internal class OAResponse {
        public const int STATUS_HTTP_ERROR = -1;
        public const int STATUS_OK = 0;
        public const int STATUS_SESSION_EXPIRED = 1;
        public const int STATUS_USER_DOES_NOT_EXIST = 8;
        public const int STATUS_GAME_DOES_NOT_EXIST = 20;

        public string Body { get; private set; }

        public HttpStatusCode HttpStatus { get; private set; }

        public int Status { get { return HttpStatus == HttpStatusCode.OK && GetJson().HasKey("status") ? GetJson().GetInt("status") : STATUS_HTTP_ERROR; }  }

        public OAResponse(string body) {
            HttpStatus = HttpStatusCode.OK;
            Body = body;
        }

        public OAResponse(HttpStatusCode statusCode) {
            HttpStatus = statusCode;
            Body = string.Empty;
        }

        public OAJson GetJson() {
            OAJson json = OAJson.FromString(Body);
            return json;
        }
    }
}
