using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenAchievements.Client {
    public class OAResult<T> : OAEmptyResult {
        public T Result { get; private set; }

        public OAResult(OAResultStatus status, T result) : base(status) {
            Result = result;
        }

        public OAResult(int oaResponseStatus, T result) : base(oaResponseStatus) {
            Result = result;
        }

        public OAResult(T result) : base(OAResultStatus.Ok) {
            Result = result;
        }
    }
}
