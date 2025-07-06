using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Extensions;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using ILogger = GamedayTracker.Interfaces.ILogger;


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
                services.AddLogging(logging => logging.ClearProviders().AddSerilog(logger));

                services.AddHostedService<BotService>()
                    .AddDiscordClient(token.Value, intents)
                    .AddCommandsExtension((options, config) =>
                    {
                        config.AddCommands(Assembly.GetExecutingAssembly());
                    });

                services.AddScoped<ITeamData, TeamDataService>();
                services.AddScoped<ITimerService, TimerService>();
                services.AddScoped<ILogger, LoggerService>();
                services.AddScoped<IGameData, GameDataService>();
                services.AddScoped<IXmlDataService, XmlDataServiceProvider>();
                services.AddScoped<IJsonDataService, JsonDataServiceProvider>();
                services.AddScoped<IPlayerData, PlayerDataServiceProvider>();
                services.AddScoped<IConfigurationData, ConfigurationDataService>();
                services.AddScoped<INewsService, NFLNewsService>();
                services.AddScoped<ICommandHelper, SlashCommandHelper>();
                services.AddScoped<IBotTimer, BotTimerDataServiceProvider>();


                #region QUARTZ
                services.AddQuartz(opt =>
                {

                });

                services.AddQuartzHostedService(q =>
                {
                    q.WaitForJobsToComplete = true;
                });

                #endregion

                services.ConfigureEventHandlers(
                    e => e.AddEventHandlers<InteractionHandler>(ServiceLifetime.Singleton)); 
                })
                .RunConsoleAsync();
            
            await Log.CloseAndFlushAsync();

            //var bot = new Bot();
            //await bot.RunAsync();

        }
    }
}
