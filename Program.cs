#region USING
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Extensions;
using GamedayTracker.Checks;
using GamedayTracker.Data;
using GamedayTracker.Enums;
using GamedayTracker.Handlers;
using GamedayTracker.Handlers.Guilds;
using GamedayTracker.Handlers.Interactions;
using GamedayTracker.Handlers.Message;
using GamedayTracker.Handlers.Modal;
using GamedayTracker.Handlers.Session;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Jobs;
using GamedayTracker.Pagination;
using GamedayTracker.Pagination.Handlers;
using GamedayTracker.Pagination.Registry;
using GamedayTracker.Repositories;
using GamedayTracker.Schedules;
using GamedayTracker.Services;
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
#endregion

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

            var logger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console(theme: theme, outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs", "bot_logs.txt"), rollingInterval: RollingInterval.Day,
                 outputTemplate: "[{Timestamp:yyyy-MM-dd hh:mm:ss.fff tt zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .WriteTo.File(new JsonFormatter(), "Data/Json/Logs/logs.json", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var host = Host.CreateDefaultBuilder()
               .UseSerilog()
               .UseConsoleLifetime()
               .ConfigureServices((context, services) =>
               {
                   services.AddHostedService<BotService>()
                    .AddDiscordClient(token.Value, intents)
                    .Configure<DiscordConfiguration>(config =>
                    {
                        config.LogUnknownEvents = false;
                    })
                    .AddCommandsExtension((context, config) =>
                    {
                        config.AddProcessor(new SlashCommandProcessor(new SlashCommandConfiguration()
                        {
                            NamingPolicy = new KebabCaseNamingPolicy()
                        }));
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
                   services.AddSingleton<IInjuryReport, InjuryReportProviderService>();
                   services.AddSingleton<DailyNumbersCacheService>();
                   services.AddDbContextFactory<BotDbContext>((context, options) =>
                   {
                       var dataService = context.GetRequiredService<IConfigurationData>();
                       var result = dataService.GetConnectionString(ConnectionStringType.Gameday);
                       if (!result.IsOk)
                           throw new InvalidOperationException("Missing connection string");

                       options.UseNpgsql(result.Value);
                   });
                   services.AddSingleton<IDailyNumbersCache>(sp =>
                       sp.GetRequiredService<DailyNumbersCacheService>());

                   services.AddSingleton<IDailyNumbersRepository>(sp =>
                       sp.GetRequiredService<DailyNumbersCacheService>());
                  
                   services.AddSingleton<TicketStore>(sp => 
                        new TicketStore(Path.Combine(AppContext.BaseDirectory, "Data", "json", "tickets.json")));

                   services.AddSingleton<ITicketProvider, TicketProviderService>();
                   services.AddSingleton<ILotteryService, DailyNumbersLotteryService>();
                   services.AddSingleton<IGlobalWinningNumberService, GlobalWinningNumberService>();
                   services.AddSingleton<IDailyNumbersPayoutRules, DefaultDailyNumbersPayoutRules>();
                   services.AddSingleton<IDailyNumbersPayoutService, DailyNumbersPayoutService>();
                   services.AddSingleton<IDmQueue>(sp =>
                        new JsonDmQueue(Path.Combine(AppContext.BaseDirectory, "dmqueue.json")));
                   services.AddHostedService<DmDispatcher>();
                   services.AddSingleton<TicketIdGenerator>();
                   services.AddSingleton<TicketCoordinator>();
                   services.AddSingleton<EspnOptions>();
                   services.AddSingleton<IEspnClient,  EspnClient>();
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
                           .StartAt(DateTimeOffset.UtcNow.AddMinutes(2))
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
                           .WithCronSchedule("0 55 23 * * ?", x => x
                                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))));
                           
                           
                   });

                   services.AddQuartzHostedService(q =>
                   {
                       q.WaitForJobsToComplete = true;
                       q.StartDelay = TimeSpan.FromMinutes(2);
                   });

                   #endregion

                   #region EVENT HANDLERS
                   services.ConfigureEventHandlers(
                       e => e.AddEventHandlers<InteractionHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<TicketInteractionHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<GuildMemberAddedEventHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<GuildAddedEventHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<ModalInteractionHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<MessageCreatedEventHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<GuildDownloadCompletedHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<GuildDeletedEventHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<SessionResumedEventHandler>(ServiceLifetime.Singleton)
                             .AddEventHandlers<ConfigureComponentInteractionHandler>());
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
