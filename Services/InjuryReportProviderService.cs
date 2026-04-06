using GamedayTracker.Enums;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL.InjuryReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class InjuryReportProviderService(HttpClient httpClient) : IInjuryReport
    {
        public async Task<Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>> GetTeamInjuryReportAsync(string endPoint)
        {
            if (endPoint == null || endPoint == "")
            {
                return Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>.Err(new SystemError<InjuryReportProviderService>
                {
                    Id = 1,
                    ErrorCode = Guid.NewGuid(),
                    ErrorMessage = $"No endpoint found for team {endPoint}",
                    CreatedBy = this,
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION
                });
            }
            else
            {
                try
                {
                    var response = await httpClient.GetAsync(endPoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<EspnInjuryResponse>(content, JsonHelper.DefaultJsonOptions);

                        data ??= new EspnInjuryResponse();
                        data.Injuries ??= [];

                        return Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>.Ok(data.Injuries);
                    }
                    else
                    {
                        return Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>.Err(new SystemError<InjuryReportProviderService>
                        {
                            Id = 2,
                            ErrorCode = Guid.NewGuid(),
                            ErrorMessage = $"Failed to fetch injury report for endpoint {endPoint}. Status code: {response.StatusCode}",
                            CreatedBy = this,
                            CreatedAt = DateTimeOffset.UtcNow,
                            ErrorType = ErrorType.WARNING
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Result<List<EspnInjury>, SystemError<InjuryReportProviderService>>.Err(new SystemError<InjuryReportProviderService>
                    {
                        Id = 3,
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = $"An error occurred while fetching injury report for endpoint {endPoint}. Exception: {ex.Message}",
                        CreatedBy = this,
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.FATAL
                    });
                }
            }
        }
    }
}
