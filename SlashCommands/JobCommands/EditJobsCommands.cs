using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Attributes;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Jobs;
using GamedayTracker.Utility;
using Quartz;
using Quartz.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.SlashCommands.JobCommands
{
    public class EditJobsCommands(ISchedulerFactory factory)
    {
        [Command("editjob")]
        [Description("Edit scheduled job")]
        public async Task EditJobs(SlashCommandContext ctx, [SlashChoiceProvider<JobChoiceProvider>] int job, [SlashChoiceProvider<TimespanChoiceProvider>] int timespan, [Parameter("interval")] int interval)
        {
            await ctx.DeferResponseAsync(true);
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var executingUser = ctx.User;
            var userId = executingUser.Id;
            var member = await ctx.Guild!.GetMemberAsync(userId);
            
            var scheduler = await factory.GetScheduler();

            if (!member.Id.Equals(524434302361010186))
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Permission Denied",
                    Description = "You do not have permission to use this command.",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(true));
                return;
            }
            else
            {
                var jobName = job switch
                {
                    1 => "RealTimeScoresJob",
                    2 => "DailyHeadlinesJob",
                    3 => "DailyStandingsJob",
                    _ => "UnknownJob"
                };

                var _interval = timespan switch
                {
                    1 => TimeSpan.FromHours(interval),
                    2 => TimeSpan.FromMinutes(interval),
                    _ => TimeSpan.FromHours(interval)
                };

                var timeSpanFormat = timespan switch
                {
                    1 => "hour(s)",
                    2 => "minute(s)",
                    _ => "hour(s)"
                };

                var jobKey = new JobKey(jobName);
                var jobDetail = await scheduler.GetJobDetail(jobKey);
                
                if (jobDetail == null)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Job Not Found",
                        Description = $"The job '{jobName}' does not exist.",
                        Color = DiscordColor.Red
                    };
                    await ctx.EditResponseAsync(new DiscordMessageBuilder().AddEmbed(embed));
                    return;
                }

                var newJobDetail = jobDetail
                    .GetJobBuilder()
                    .WithDescription($"{jobName} | interval: {timeSpanFormat}")
                    .Build();

                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                foreach (var oldTrigger in triggers)
                {
                    
                    var newTrigger = oldTrigger
                        .GetTriggerBuilder()
                        .ForJob(jobKey)
                        .WithIdentity($"{jobName}-trigger")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithInterval(_interval)
                            .RepeatForever())
                        .Build();

                    await scheduler.RescheduleJob(oldTrigger.Key, newTrigger);
                }

                DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent("## Job Edited"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"**Job**: {newJobDetail!.JobType.Name}"),
                    new DiscordTextDisplayComponent($"**Description**: {newJobDetail!.Description}"),
                    new DiscordTextDisplayComponent($"**Interval**: {interval} {timeSpanFormat}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {timestamp}"),
                                      new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);

                await ctx.EditResponseAsync(new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container));

            }
        }
    }
}
