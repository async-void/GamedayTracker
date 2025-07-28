using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.Extensions;
using GamedayTracker.Checks;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Jobs;
using GamedayTracker.Models;
using GamedayTracker.Schedules;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using Log = Serilog.Log;


namespace GamedayTracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            Console.WriteLine("\r\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(@"


         ▄████  ▄▄▄       ███▄ ▄███▓▓█████ ▓█████▄  ▄▄▄     ▓██   ██▓   ▄▄▄█████▓ ██▀███   ▄▄▄       ▄████▄   ██ ▄█▀▓█████  ██▀███  
        ██▒ ▀█▒▒████▄    ▓██▒▀█▀ ██▒▓█   ▀ ▒██▀ ██▌▒████▄    ▒██  ██▒   ▓  ██▒ ▓▒▓██ ▒ ██▒▒████▄    ▒██▀ ▀█   ██▄█▒ ▓█   ▀ ▓██ ▒ ██▒
       ▒██░▄▄▄░▒██  ▀█▄  ▓██    ▓██░▒███   ░██   █▌▒██  ▀█▄   ▒██ ██░   ▒ ▓██░ ▒░▓██ ░▄█ ▒▒██  ▀█▄  ▒▓█    ▄ ▓███▄░ ▒███   ▓██ ░▄█ ▒
       ░▓█  ██▓░██▄▄▄▄██ ▒██    ▒██ ▒▓█  ▄ ░▓█▄   ▌░██▄▄▄▄██  ░ ▐██▓░   ░ ▓██▓ ░ ▒██▀▀█▄  ░██▄▄▄▄██ ▒▓▓▄ ▄██▒▓██ █▄ ▒▓█  ▄ ▒██▀▀█▄  
       ░▒▓███▀▒ ▓█   ▓██▒▒██▒   ░██▒░▒████▒░▒████▓  ▓█   ▓██▒ ░ ██▒▓░     ▒██▒ ░ ░██▓ ▒██▒ ▓█   ▓██▒▒ ▓███▀ ░▒██▒ █▄░▒████▒░██▓ ▒██▒
        ░▒   ▒  ▒▒   ▓▒█░░ ▒░   ░  ░░░ ▒░ ░ ▒▒▓  ▒  ▒▒   ▓▒█░  ██▒▒▒      ▒ ░░   ░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ░▒ ▒  ░▒ ▒▒ ▓▒░░ ▒░ ░░ ▒▓ ░▒▓░
         ░   ░   ▒   ▒▒ ░░  ░      ░ ░ ░  ░ ░ ▒  ▒   ▒   ▒▒ ░▓██ ░▒░        ░      ░▒ ░ ▒░  ▒   ▒▒ ░  ░  ▒   ░ ░▒ ▒░ ░ ░  ░  ░▒ ░ ▒░
       ░ ░   ░   ░   ▒   ░      ░      ░    ░ ░  ░   ░   ▒   ▒ ▒ ░░       ░        ░░   ░   ░   ▒   ░        ░ ░░ ░    ░     ░░   ░ 
             ░       ░  ░       ░      ░  ░   ░          ░  ░░ ░                    ░           ░  ░░ ░      ░  ░      ░  ░   ░     
                                                 ░                ░ ░                                    ░                               
            

");
            Console.ResetColor();

            var configService = new ConfigurationDataService();
            var botTimerService = new BotTimerDataServiceProvider();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs"));
            await botTimerService.WriteTimestampToTextAsync();

            if (!token.IsOk)
            {
                Log.Information($"Error retrieving token: {token.Error.ErrorMessage}");
                return;
            }

            var intents = TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.All;

            var logger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs", "bot_logs.txt"), rollingInterval: RollingInterval.Day,
                 outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .CreateLogger();
           
            await Host.CreateDefaultBuilder()
                .UseSerilog()
                .UseConsoleLifetime()
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<BotService>()
                        .AddDiscordClient(token.Value, intents)
                        .AddCommandsExtension((context, config) =>
                        {
                            config.AddCommands(Assembly.GetExecutingAssembly());
                            config.AddCheck<RequireRoleCheck>();
                        });
                    
                    services.AddLogging(logging => logging.ClearProviders().AddSerilog(logger));
                    services.AddScoped<ITeamData, TeamDataService>();
                    services.AddScoped<ITimerService, TimerService>();
                    services.AddScoped<IGameData, GameDataService>();
                    services.AddScoped<IXmlDataService, XmlDataServiceProvider>();
                    services.AddScoped<IJsonDataService, JsonDataServiceProvider>();
                    services.AddScoped<IPlayerData, PlayerDataServiceProvider>();
                    services.AddScoped<IConfigurationData, ConfigurationDataService>();
                    services.AddScoped<INewsService, NFLNewsService>();
                    services.AddScoped<ICommandHelper, SlashCommandHelper>();
                    services.AddScoped<IBotTimer, BotTimerDataServiceProvider>();
                    services.AddScoped<IEvaluator, RealTimeScoresModeEvaluatorService>();  
                    services.AddScoped<DailyHeadlinesScheduler>();

                    #region QUARTZ
                    services.AddQuartz(q =>
                    {
                        var rtJobKey = new JobKey("RealTimeScoresJob");
                        var headlinesJobKey = new JobKey("DailyHeadlinesJob");
                        var dailyStandingsJobKey = new JobKey("DailyStandingsJob");

                        q.AddJob<RealTimeScoresJob>(opts => opts.WithIdentity(rtJobKey)
                        .WithDescription("get realtime scores : user-defined intervals").Build());

                        q.AddTrigger(opts => opts
                            .ForJob(rtJobKey)
                            .WithIdentity("RealTimeScores-trigger")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithInterval(TimeSpan.FromHours(4))
                                .RepeatForever().Build()));

                        q.AddJob<DailyHeadlineJob>(opts => opts.WithIdentity(headlinesJobKey)
                        .WithDescription("get daily headlines : 24 hour interval").Build());

                        q.AddTrigger(opts => opts
                            .ForJob(headlinesJobKey)
                            .WithIdentity("DailyHeadlines-trigger")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithInterval(TimeSpan.FromHours(4))
                                .RepeatForever().Build()));

                        q.AddJob<DailyStandingsJob>(opts => opts.WithIdentity(dailyStandingsJobKey)
                        .WithDescription("get daily standings : 12 hour interval").Build());

                        q.AddTrigger(opts => opts
                            .ForJob(dailyStandingsJobKey)
                            .WithIdentity("DailyStandings-trigger")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithInterval(TimeSpan.FromHours(4))
                                .RepeatForever().Build()));
                    });

                    services.AddQuartzHostedService(q =>
                    {
                        q.WaitForJobsToComplete = true;
                    });

                    #endregion

                    #region CONFIGURE EVENT HANDLERS
                    services.ConfigureEventHandlers(
                        e => e.AddEventHandlers<InteractionHandler>(ServiceLifetime.Singleton));
                    services.ConfigureEventHandlers(

                    #region MESSAGE EVENT HANDLERS
                        e => e.HandleMessageCreated((sender, args) =>
                        {
                            return Task.CompletedTask;
                        })
                        .HandleMessageDeleted(async (sender, args) =>
                        {
                            var logChnl = await sender.GetChannelAsync(1384436855524692048);
                            if (logChnl is { } channel)
                            {
                                var message = "";
                                if (args.Message.Embeds.Count > 0)
                                    message = "message was embed, cannot retrieve message content";
                                else
                                    message = args.Message.Content.Length > 0 ? args.Message.Content : message;
                                DiscordComponent[] comps =
                                [
                                    new DiscordTextDisplayComponent($"{args.Message.Author} deleted message: [{message}] from: {args.Message.Channel!.Name}:``{args.Message.Channel.Id}``"),
                                    new DiscordTextDisplayComponent($"Guild: {args.Guild.Name}:``{args.Guild.Id}``"),
                                ];
                                var container = new DiscordContainerComponent(comps, false, DiscordColor.Red);
                                var embed = new DiscordMessageBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);
                                await logChnl.SendMessageAsync(embed);
                            }
                        })
                        #endregion

                        #region GUILD EVENT HANDLERS

                        #region GUILD CREATED
                        .HandleGuildCreated(async (sender, args) =>
                        {
                            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            var jsonService = sender.ServiceProvider.GetRequiredService<IJsonDataService>();

                            var guild = new Guild()
                            {
                                GuildId = args.Guild.Id.ToString(),
                                GuildName = args.Guild.Name,
                                GuildOwnerId = args.Guild.OwnerId.ToString(),
                                DateAdded = DateTimeOffset.UtcNow,
                                IsDailyHeadlinesEnabled = true,
                                IsRealTimeScoresEnabled = true,
                                ReceiveSystemMessages = true,
                                NotificationChannelId = args.Guild.GetDefaultChannel()!.Id.ToString()

                            };
                            var supportChnl = await sender.GetChannelAsync(888659367824601160);
                            var guilds = sender.Guilds.Values;

                            var newChnl = args.Guild.GetDefaultChannel();
                            if (newChnl is { } chnl)
                            {
                                DiscordComponent[] components =
                                [
                                    new DiscordTextDisplayComponent("## Welcome to Gameday Tracker!"),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent("Use the `help button` to get started!"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "helpId", "Help")),
                                    new DiscordSeparatorComponent(true),
                                    new DiscordSectionComponent(new DiscordTextDisplayComponent("Headlines and Realtime Scores are enabled by default!"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "settingsId", "Settings")),
                                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                                    new DiscordSectionComponent(
                                        new DiscordTextDisplayComponent($"Powered by GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                                ];

                                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                                var embed = new DiscordMessageBuilder()
                                    .EnableV2Components()
                                    .AddContainerComponent(container);
                                await chnl.SendMessageAsync(embed);
                            }

                            await supportChnl.SendMessageAsync(
                                $"``New Guild Added: <t:{unixTimestamp}:F> {args.Guild.Name}:({args.Guild.Id}) - Total Guilds: {guilds.Count()}``");
                            var guildResult = await jsonService.WriteGuildToJsonAsync(guild);

                            if (guildResult.IsOk)
                                Log.Information($"New Guild Added: {args.Guild.Name} ({args.Guild.Id}) - Total Guilds: {guilds.Count()}");
                            else
                                Log.Error($"GUild Added to Discord: {args.Guild.Name} ({args.Guild.Id}) - Total Guilds: {guilds.Count()}\r\n" +
                                    $"but unable to write guild info to json file: Error Message - {guildResult.Error.ErrorMessage}");



                        })
                        #endregion

                        #region GUILD DELETED
                        .HandleGuildDeleted(async (sender, args) =>
                        {
                            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            var jsonService = sender.ServiceProvider.GetRequiredService<IJsonDataService>();
                            var guildResult = await jsonService.GetGuildFromJsonAsync(args.Guild.Id.ToString());
                            if (guildResult.IsOk)
                            {
                                var removedResult = await jsonService.RemoveGuildDataAsync(guildResult.Value.GuildId);
                                var guildOwner = await args.Guild.GetMemberAsync(args.Guild.OwnerId);
                                if (removedResult.IsOk)
                                {
                                    await guildOwner.SendMessageAsync(
                                        $"``Gameday Tracker has been removed at: [<t:{unixTimestamp}:F>]``\r\nyou will no longer be receiving Daily Headlines or Realtime Scores Updates");
                                    Log.Information($"GamedayTracker has been removed from: {args.Guild.Name}");
                                }
                                else
                                {
                                    await args.Guild.GetDefaultChannel()!.SendMessageAsync(
                                        $"``Error removing Gameday Tracker from {args.Guild.Name} ({args.Guild.Id}) at: [<t:{unixTimestamp}:F>]``\r\n{removedResult.Error.ErrorMessage}");
                                    Log.Error($"Error removing guild {args.Guild.Name} ({args.Guild.Id}): {removedResult.Error.ErrorMessage}");
                                }

                            }
                        })
                        #endregion

                        #region GUILD DOWNLOAD COMPLETED
                        .HandleGuildDownloadCompleted(async (sender, args) =>
                        {
                            var count = sender.Guilds.Count;
                            await sender.UpdateStatusAsync(new DiscordActivity($"Scores in {count} Servers", DiscordActivityType.Watching));
                        })
                        #endregion

                        #endregion
                    );
                    #endregion

                })
                
                .RunConsoleAsync();
            
            await Log.CloseAndFlushAsync();

        }
    }
}
