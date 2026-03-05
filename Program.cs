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
using GamedayTracker.Pagination;
using GamedayTracker.Pagination.Handlers;
using GamedayTracker.Pagination.Registry;
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

namespace GamedayTracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            Console.WriteLine("\r\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("""


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
            

""");
            Console.ResetColor();
           
            var configService = new ConfigurationDataService();
            var botTimerService = new BotTimerDataServiceProvider();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs"));
            await botTimerService.WriteTimestampToTextAsync();

            var intents = TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.All;
            var theme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\x1b[37m",
                [ConsoleThemeStyle.Number] = "\x1b[33m",
                [ConsoleThemeStyle.String] = "\x1b[38;5;208m",
                [ConsoleThemeStyle.LevelInformation] = "\x1b[32m",
                [ConsoleThemeStyle.LevelError] = "\x1b[31m"
            });

            var logger = Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console(theme: theme, outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs", "bot_logs.txt"), rollingInterval: RollingInterval.Day,
                 outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
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
                   services.AddMemoryCache();
                   services.AddSingleton<HttpClient>();
                   services.AddLogging(logging => logging.ClearProviders().AddSerilog(logger));
                   services.AddScoped<ITeamData, TeamDataService>();
                   services.AddScoped<ITimerService, TimerService>();
                   services.AddScoped<IGameData, GameDataService>();
                   services.AddScoped<IDiscordEmbedService, DiscordEmbedService>();
                   services.AddScoped<IXmlDataService, XmlDataServiceProvider>();
                   services.AddScoped<IJsonDataService, JsonDataServiceProvider>();
                   services.AddScoped<IPlayerData, PlayerDataServiceProvider>();
                   services.AddScoped<IConfigurationData, ConfigurationDataService>();
                   services.AddScoped<INewsService, NFLNewsService>();
                   services.AddScoped<ICommandHelper, SlashCommandHelper>();
                   services.AddScoped<IBotTimer, BotTimerDataServiceProvider>();
                   services.AddScoped<IEvaluator, RealTimeScoresModeEvaluatorService>();
                   services.AddScoped<IBetting, BettingDataServiceProvider>();
                   services.AddSingleton<IGuildMemberService, GuildMemberService>();
                   services.AddSingleton<ILotteryService, DailyNumbersLotteryService>();
                   services.AddScoped<DailyHeadlinesScheduler>();
                   services.AddSingleton<ScoreboardPaginationHandler>();
                   services.AddSingleton<TeamStatsPaginationHandler>(); 
                   services.AddSingleton<MemberBetsPaginationHandler>();
                   services.AddSingleton<IDailyNumbersCache, DailyNumbersCacheService>();
                   services.AddSingleton<DailyNumbersCacheService>();
                   
                   services.AddSingleton<IDailyNumbersCache>(sp =>
                       sp.GetRequiredService<DailyNumbersCacheService>());

                   services.AddSingleton<IDailyNumbersRepository>(sp =>
                       sp.GetRequiredService<DailyNumbersCacheService>());
                  
                   services.AddSingleton<ILotteryService, DailyNumbersLotteryService>();
                   services.AddSingleton<IGlobalWinningNumberService, GlobalWinningNumberService>();
                   services.AddSingleton<IDailyNumbersPayoutRules, DefaultDailyNumbersPayoutRules>();
                   services.AddSingleton<IDailyNumbersPayoutService, DailyNumbersPayoutService>();
                   services.AddSingleton<IDmQueue>(sp =>
                        new JsonDmQueue(Path.Combine(AppContext.BaseDirectory, "dmqueue.json")));
                   services.AddHostedService<DmDispatcher>();
                  // services.AddHostedService<NotificationDispatcher>();

                   #region QUARTZ
                   services.AddQuartz(q =>
                   {
                       var scoresInterval = 12;
                       var headlinesInterval = 24;
                       var standingsInterval = 24;

                       var rtJobKey = new JobKey("RealTimeScoresJob");
                       var headlinesJobKey = new JobKey("DailyHeadlinesJob");
                       var dailyStandingsJobKey = new JobKey("DailyStandingsJob");
                       var updateBotStatusJobKey = new JobKey("UpdateBotStatusJob");
                       var bettingWatcherJobKey = new JobKey("BetsWatcherJob");
                       var dailyNumbersJobKey = new JobKey("DailyNumbersLotteryJob");

                       q.AddJob<RealTimeScoresJob>(opts => opts.WithIdentity(rtJobKey)
                       .WithDescription("get realtime scores : user defined intervals | min:1 minute | max - 24 hour").Build());

                       q.AddTrigger(opts => opts
                           .ForJob(rtJobKey)
                           .WithIdentity("RealTimeScores-trigger")
                           .StartNow()
                           .WithSimpleSchedule(x => x
                               .WithInterval(TimeSpan.FromHours(scoresInterval))
                               .RepeatForever().Build()));

                       q.AddJob<DailyHeadlineJob>(opts => opts.WithIdentity(headlinesJobKey)
                       .WithDescription($"get daily headlines : {headlinesInterval} hour interval").Build());

                       q.AddTrigger(opts => opts
                           .ForJob(headlinesJobKey)
                           .WithIdentity("DailyHeadlines-trigger")
                           .StartNow()
                           .WithSimpleSchedule(x => x
                               .WithInterval(TimeSpan.FromHours(headlinesInterval))
                               .RepeatForever().Build()));

                       q.AddJob<DailyStandingsJob>(opts => opts.WithIdentity(dailyStandingsJobKey)
                       .WithDescription($"get daily standings : {standingsInterval} hour interval").Build());

                       q.AddTrigger(opts => opts
                           .ForJob(dailyStandingsJobKey)
                           .WithIdentity("DailyStandings-trigger")
                           .StartNow()
                           .WithSimpleSchedule(x => x
                               .WithInterval(TimeSpan.FromHours(standingsInterval))
                               .RepeatForever().Build()));

                       q.AddJob<UpdateBotStatusJob>(opts => opts.WithIdentity(updateBotStatusJobKey)
                           .WithDescription("update bot status : 10 minute interval").Build());
                       q.AddTrigger(opts => opts
                           .ForJob(updateBotStatusJobKey)
                           .WithIdentity("UpdateBotStatus-trigger")
                           .StartAt(DateTimeOffset.UtcNow.AddMinutes(10))
                           .WithSimpleSchedule(x => x
                               .WithInterval(TimeSpan.FromMinutes(10))
                               .RepeatForever().Build()));

                       q.AddJob<BetsWatcherJob>(opts => opts.WithIdentity(bettingWatcherJobKey)
                          .WithDescription("Bet Watcher Job : 12 hour interval").Build());
                       q.AddTrigger(opts => opts
                           .ForJob(bettingWatcherJobKey)
                           .WithIdentity("BetsWatcher-trigger")
                           .StartAt(DateTimeOffset.UtcNow.AddMinutes(10))
                           .WithSimpleSchedule(x => x
                               .WithInterval(TimeSpan.FromHours(12))
                               .RepeatForever().Build()));

                       q.AddJob<DailyNumbersLotteryJob>(opts => opts
                         .WithIdentity(dailyNumbersJobKey)
                         .WithDescription("Daily Numbers Job: 24 hour interval").Build());
                       q.AddTrigger(opts => opts
                           .ForJob(dailyNumbersJobKey)
                           .WithIdentity("DailyNumbers-trigger")
                           .WithCronSchedule("0 0 0 * * ?", x => x
                                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))));
                           
                           
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

                   #endregion

                    #region INTERACTION EVENT HANDLERS
                    .HandleInteractionCreated(async (sender, args) =>
                    {
                        var userBet = args.Interaction.Data.CustomId;
                        var test = "";
                    })
                    #endregion

                    #region GUILD EVENT HANDLERS

                    #region SESSION RESUMED
                    .HandleSessionResumed(async (sender, args) =>
                    {
                        var guildCount = sender.Guilds.Count;
                        var userCount = sender.Guilds.Values.Sum(g => g.Members.Count);
                        var statusMessage = $"Serving {guildCount} Servers";
                        await sender.UpdateStatusAsync(new DiscordActivity($"Scores in {guildCount} Servers", DiscordActivityType.Watching));
                        var logChnl = await sender.GetChannelAsync(1384436855524692048);
                        await logChnl.SendMessageAsync(
                            $"Updated bot status: {statusMessage} with a total of ``{userCount}`` users.");
                        logger.Information($"Session Resumed");
                    })
                    #endregion

                    #region GUILD CREATED
                    .HandleGuildCreated(async (sender, args) =>
                    {
                        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        var jsonService = sender.ServiceProvider.GetRequiredService<IJsonDataService>();

                        var guild = new Guild()
                        {
                            GuildId = args.Guild.Id,
                            GuildName = args.Guild.Name,
                            GuildOwnerId = args.Guild.OwnerId,
                            DateAdded = DateTimeOffset.UtcNow,
                            IsDailyHeadlinesEnabled = true,
                            IsRealTimeScoresEnabled = true,
                            ReceiveSystemMessages = true,
                            NotificationChannelId = args.Guild.GetDefaultChannel()!.Id.ToString(),
                            DiscordMembers = args.Guild.Members.ToDictionary()

                        };
                        var supportChnl = await sender.GetChannelAsync(1384436855524692048);
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
                        var guildOwner = await args.Guild.GetMemberAsync(args.Guild.OwnerId);
                        await supportChnl.SendMessageAsync(
                            $"Guild Added: <t:{unixTimestamp}:R> ``{args.Guild.Name}:({args.Guild.Id}) - Total Guilds: {guilds.Count()}``\r\n" +
                            $"``OwnerId: {guildOwner.Id} Owner Membername: {guildOwner.Username}``");
                        var guildResult = await jsonService.WriteGuildToJsonAsync(guild);

                        if (guildResult.IsOk)
                            logger.Information($"Guild Added: {args.Guild.Name} ({args.Guild.Id}) - Total Guilds: {guilds.Count()}");
                        else
                            logger.Error($"GUild Added to Discord: {args.Guild.Name} ({args.Guild.Id}) - Total Guilds: {guilds.Count()}\r\n" +
                                $"but unable to write guild info to json file: Error Message - {guildResult.Error.ErrorMessage}");



                    })
                    #endregion

                    #region GUILD DELETED
                    .HandleGuildDeleted(async (sender, args) =>
                    {
                        var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
                        var jsonService = sender.ServiceProvider.GetRequiredService<IJsonDataService>();
                        var guildResult = await jsonService.GetGuildFromJsonAsync(args.Guild.Id);
                        if (guildResult.IsOk)
                        {
                            var removedResult = await jsonService.RemoveGuildDataAsync(guildResult.Value.GuildId);
                            var guildOwner = await args.Guild.GetMemberAsync(args.Guild.OwnerId);
                            if (removedResult.IsOk)
                            {
                                await guildOwner.SendMessageAsync(
                                    $"``Gameday Tracker has been removed at: [{unixTimestamp}]``\r\nyou will no longer be receiving Daily Headlines or Realtime Scores Updates");
                                logger.Information($"GamedayTracker has been removed from: {args.Guild.Name}");
                            }
                            else
                            {
                                await args.Guild.GetDefaultChannel()!.SendMessageAsync(
                                    $"``Error removing Gameday Tracker from {args.Guild.Name} ({args.Guild.Id}) at: [{unixTimestamp}]``\r\n{removedResult.Error.ErrorMessage}");
                                logger.Error($"Error removing guild {args.Guild.Name} ({args.Guild.Id}): {removedResult.Error.ErrorMessage}");
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

               }).Build();
               RegisterPaginationHandlers(host.Services);
            await host.RunAsync();
            await Serilog.Log.CloseAndFlushAsync();

        }

        private static void RegisterPaginationHandlers(IServiceProvider provider)
        {
            PaginationHandlerRegistry.Register<NFLScoreboardPaginationData>(
                provider.GetRequiredService<ScoreboardPaginationHandler>());

            PaginationHandlerRegistry.Register<TeamStatsPaginationData>(
                provider.GetRequiredService<TeamStatsPaginationHandler>());

            PaginationHandlerRegistry.Register<MemberBetsPaginationData>(
                provider.GetRequiredService<MemberBetsPaginationHandler>());
        }
    }
}
