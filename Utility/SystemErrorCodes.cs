using GamedayTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Utility
{
    public class SystemErrorCodes
    {
       
        private static readonly Dictionary<ErrorCode, string> ErrorMessages = new()
        {
            { ErrorCode.FileNotFound, "File not found." },
            { ErrorCode.MemberNotFound, "Member not found." },
            { ErrorCode.ResourceNotFound, "Resource not found." },
            { ErrorCode.RoleNotFound, "Role not found." },
            { ErrorCode.UnauthorizedAccess, "Access Denied." },
            { ErrorCode.DatabaseError, "Database error occurred." },
            { ErrorCode.OperationFailed, "Operation execution failed." },
            { ErrorCode.ConfigurationError, "Configuration error." },
            { ErrorCode.InvalidInput, "Invalid input provided." },
            { ErrorCode.InvalidCredentials, "Invalid credentials provided." },
            { ErrorCode.TimeoutError, "Operation timed out." },
            { ErrorCode.ServiceUnavailable, "Service is currently unavailable." },
            { ErrorCode.RateLimitExceeded, "Rate limit exceeded. Please try again later." },
            { ErrorCode.CommandNotFound, "Command not found." },
            { ErrorCode.FeatureNotImplemented, "Feature not implemented." },
            { ErrorCode.NetworkError, "Network error occurred." },
            { ErrorCode.UnexpectedError, "An unexpected error occurred." }
          
        };

        public static string GetErrorMessage(ErrorCode errorCode)
        {
            return ErrorMessages.TryGetValue(errorCode, out var message) ? message : "Unknown error occurred.";
        }
    }
}
