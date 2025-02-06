using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.Services
{
    public class InteractionDataProviderService : IInteractionProvider
    {
        public Result<string, SystemError<InteractionDataProviderService>> ParseButtonId(string buttonId)
        {
            var result = buttonId switch
            {
                "scoreBoardBtn" => "ScoreBoard",
                "standingsBtn" => "Standings",
                "userSettingsBtn" => "User-Settings",
                "newsBtn" => "News",
                _ => "Not Found"
            };

            if (result == "Not Found")
                return Result<string, SystemError<InteractionDataProviderService>>.Err(new SystemError<InteractionDataProviderService>
                {
                    ErrorMessage = "Not Found",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });

            return Result<string, SystemError<InteractionDataProviderService>>.Ok(result);
        }
    }
}
