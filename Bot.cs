using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using ILogger = GamedayTracker.Interfaces.ILogger;

namespace GamedayTracker
{
    public class Bot
    {
        public async Task RunAsync()
        { 
            var configService = new ConfigurationDataService();
            var botTimerService = new BotTimerDataServiceProvider();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs"));
            await botTimerService.WriteTimestampToTextAsync();

            //Log.Logger = (Serilog.ILogger)new LoggerConfiguration()
            //                   .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
            //                   .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs", "bot_logs.txt"), rollingInterval: RollingInterval.Day,
            //                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
            //                   .CreateLogger();

            var dBuilder = DiscordClientBuilder.CreateDefault(token.Value, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.All);
            
            var configuration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Error)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                                 outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "TextFiles", "Logs", "trace_logs.txt"), rollingInterval: RollingInterval.Day,
                 outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {SourceContext} {Level:u3}] {Message:lj}{NewLine}")
                .CreateLogger();


            #region CONFIGURE SERVICES
            dBuilder.ConfigureServices(services =>
            {
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
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(configuration, dispose: true);
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                });
            });
            #endregion


            dBuilder.UseInteractivity();
            dBuilder.UseZlibCompression();

            #region USE COMMANDS
            dBuilder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(Assembly.GetExecutingAssembly());

                TextCommandProcessor textCommandProcessor = new(new()
                {
                    PrefixResolver = new DefaultPrefixResolver(true, prefix.Value).ResolvePrefixAsync,
                });

                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration
            {
                RegisterDefaultCommandProcessors = true,
                DebugGuildId = 0,

            });
            #endregion

            #region EVENT HANDLERS

            dBuilder.ConfigureEventHandlers(b => b.AddEventHandlers<InteractionHandler>(ServiceLifetime.Singleton));
           // dBuilder.ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Debug));

            dBuilder.ConfigureEventHandlers(
                m => m.HandleMessageCreated(async (s, e) =>
                    {
                        //if (e.Message!.Author!.IsBot)
                        //    return;
                        //if (e.Message!.Author!.Id.Equals(1076279184667721859))
                        //{
                        //    if (e.Guild.Id.Equals(764184337620140062) && e.Message!.Channel!.Id.Equals(1076279102841045093))
                        //    {
                        //        var channel = await s.GetChannelAsync(1367596778362241086); 
                        //        await channel.SendMessageAsync("reload");
                        //    }
                        //}

                        //if (e.Message.Author!.IsBot && e.Message.Content.Equals("reload"))
                        //{
                        //    await e.Message.RespondAsync("``documentation reloaded``");
                        //    Console.ForegroundColor = ConsoleColor.Yellow;
                        //    Console.Write($"[{DateTimeOffset.UtcNow}] [Gameday Tracker] ");
                        //    Console.ForegroundColor = ConsoleColor.Magenta;
                        //    Console.Write($"[INFO] ");
                        //    Console.ForegroundColor = ConsoleColor.DarkGray;
                        //    Console.Write( $"Documents Reloaded!");
                        //    Console.WriteLine();
                        //    Console.ResetColor();
                        //    return;
                        //}
                        
                        //if (e.Message.Content.Contains("help"))
                        //{
                        //    var user = e.Author;
                        //    await e.Channel.SendMessageAsync(
                        //        "I noticed you needed some help.....please use the ``/help`` command to see a list of help topics");
                        //    return;
                        //}
                    })

                #region SOCKET EVENTS
                .HandleSocketClosed(async (s, e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow}");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($" ERROR ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"] ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"Gateway Connection Closed. Reconnecting...");
                        Console.WriteLine();
                        await s.ReconnectAsync();
                    })
                .HandleSocketOpened(async (s, e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow} ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"] ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"Gateway Connection Opened. Connected to Discord.");
                        Console.WriteLine();
                    })
                #endregion

                #region ZOMIED
                    .HandleZombied(async (s, e) =>
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"[{DateTime.UtcNow} ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write($"ERROR ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"] ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("[Gameday Tracker] ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"Zombied Gateway Connection Detected. Reconnecting...");
                            Console.WriteLine();
                            await s.ReconnectAsync();
                        })
                    #endregion

                #region DOWNLOAD COMPLETE
                    .HandleGuildDownloadCompleted(async (e, s) =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("] ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Guild Download Complete.");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("] ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Connection Established");
                        Console.WriteLine();

                    })

                   #endregion

                #region SESSION CREATED
                    .HandleSessionCreated(async (s,e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("] "); ;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Shaking Hands With Discord ");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("] ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[DiscordClient] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Session Started");
                        Console.WriteLine();
                    })
                   #endregion

                #region GUILD CREATED
                    .HandleGuildCreated(async (s, e) =>
                    {
                        
                        //TODO: need to write the guild to the json file.
                        var g = new Guild()
                        {
                            DateAdded = DateTime.UtcNow,
                            GuildId = (long)e.Guild.Id,
                            GuildName = e.Guild.Name,
                            GuildOwnerId = (long)e.Guild.OwnerId,
                            NotificationChannelId = (long)e.Guild.SystemChannelId!
                        };
                            

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.UtcNow} ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" INF");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"] ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[Gameday Tracker]");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"Guild Created | Guild ID: {e.Guild.Id} | Guild Name: {e.Guild.Name}");
                        Console.WriteLine();

                        //TODO: send this message to the support server
                        var supportServer = await s.GetGuildAsync(1384428811805921301);
                        var sysChnl = await supportServer.GetChannelAsync(1384436855524692048);
                        await sysChnl.SendMessageAsync($"New Guild Created: {e.Guild.Name} | ID: {e.Guild.Id} | Owner: <@{e.Guild.OwnerId}>");
                    })
                   #endregion

                );


            #endregion

            
            var status = new DiscordActivity("Game-Day", DiscordActivityType.Watching);
            var client = dBuilder.Build();
            dBuilder.SetReconnectOnFatalGatewayErrors();
            await client.ConnectAsync(status, DiscordUserStatus.Online, DateTimeOffset.UtcNow);

            //await Task.Delay(-1);
            while(Console.ReadLine() == "test")
            {
                Console.WriteLine("hello world");
            }
        }

    }
}
