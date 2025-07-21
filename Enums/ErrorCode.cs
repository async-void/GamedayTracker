using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Enums
{
    public enum ErrorCode
    {
        FileNotFound = 1001,
        MemberNotFound = 1002,
        GuildNotFound = 1003,
        InvalidInput = 1004,
        DatabaseError = 1005,
        UnauthorizedAccess = 1006,
        OperationFailed = 1007,
        ResourceNotFound = 1008,
        InvalidCredentials = 1009,
        TimeoutError = 1010,
        ServiceUnavailable = 1011,
        RoleNotFound = 1012,
        ConfigurationError = 1013,
        RateLimitExceeded = 1014,
        CommandNotFound = 1015,
        FeatureNotImplemented = 1016,
        NetworkError = 1017,
        UnexpectedError = 1018,
    }
}
