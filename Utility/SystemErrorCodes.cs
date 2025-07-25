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

        private static readonly Dictionary<Guid, string> ErrorCodes = new Dictionary<Guid, string>
        {
            { Guid.Parse("f416e176-85b0-4f94-b172-8dc8f084242e"), "Missing permissions [Kick Member]" },
            { Guid.Parse("6b24c3ac-0a78-49fd-9c6e-40d749e6559e"), "Command must be run in a text channel, not DM." },
            { Guid.Parse("d9b3f7e1-6f02-4aad-b591-992ad2cbdd91"), "Bot failed to retrieve guild configuration." },
            { Guid.Parse("3a5e8f47-9a7d-404a-bf4d-41f16f5b3679"), "Failed to send DM — user might have DMs disabled." },
            { Guid.Parse("bf3a4281-2d3b-4412-a4d8-7f22869dc74c"), "Required role not found in the server." },
            { Guid.Parse("c208f76f-037d-4f39-9144-cb82e56df6f7"), "Message too long to send — exceeds Discord limits." },
            { Guid.Parse("e1f54bcb-3e9c-4784-af4c-44b97af1d0e2"), "Interaction expired — please try again." },
            { Guid.Parse("94807acb-8869-4648-a05d-c258af989e2f"), "User not in voice channel." },
            { Guid.Parse("46665a87-d1d7-43fd-bd18-d04502b86a1d"), "Voice channel not found." },
            { Guid.Parse("64cd2f70-3614-4bd5-a43e-3bd250ebde1b"), "Bot lacks permission to join voice channel." },
            { Guid.Parse("197e2742-7a3e-4f32-b253-38e5a5df7609"), "Slash command failed to register." },
            { Guid.Parse("b9df9796-772b-4b67-98a9-17de2c7e16fe"), "Too many arguments supplied — check command help." },
            { Guid.Parse("56335558-e5bc-4b39-bf30-88f901cd318e"), "Embed image URL invalid or unreachable." },
            { Guid.Parse("a48f5af7-7e01-4ae9-81d6-869e11953bb3"), "Missing bot token configuration." },
            { Guid.Parse("2a35e0b1-3f87-4c7a-8e6d-5bc1cf301b1f"), "Guild not found" },
            { Guid.Parse("ebef597c-1e32-4f4e-844a-9742059ddf47"), "Could not resolve emoji in the reaction list." },
            { Guid.Parse("35d9e8da-90a3-4e79-b194-99dd810ed763"), "File upload failed due to size limit." },
            { Guid.Parse("bb2a8e58-9f6b-4ec1-9d8e-d2f6732cbe62"), "User interaction button mismatch or outdated." },
            { Guid.Parse("ff80cb50-a3dc-4de7-a9df-0c58f395b0f0"), "Command execution interrupted by shutdown signal." },
            { Guid.Parse("f416e176-85b0-4f94-b172-8dc8f084242e"), "File Not Found" },
            { Guid.Parse("6b24c3ac-0a78-49fd-9c6e-40d749e6559e"), "Member Not Found" },
            { Guid.Parse("d9b3f7e1-6f02-4aad-b591-992ad2cbdd91"), "Invalid Channel Type" },
            { Guid.Parse("3a5e8f47-9a7d-404a-bf4d-41f16f5b3679"), "Database Connection Failed" },
            { Guid.Parse("bf3a4281-2d3b-4412-a4d8-7f22869dc74c"), "API Rate Limit Exceeded" },
            { Guid.Parse("c208f76f-037d-4f39-9144-cb82e56df6f7"), "Invalid Command Usage" },
            { Guid.Parse("e1f54bcb-3e9c-4784-af4c-44b97af1d0e2"), "Permission Denied" },
            { Guid.Parse("94807acb-8869-4648-a05d-c258af989e2f"), "Unknown Error Occurred" }
        };

        public static string GetErrorMessage(Guid errorCode)
        {
            return ErrorCodes.TryGetValue(errorCode, out var message) ? message : "Unknown error occurred.";
        }
    }
}
