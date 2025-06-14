﻿using System.Reflection;
using ChalkDotNET;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Helpers;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.DependencyInjection;
using ILogger = GamedayTracker.Interfaces.ILogger;

namespace GamedayTracker
{
    public class Bot
    {
        private readonly TimerService timerService = new TimerService();

        public async Task RunAsync()
        { 
            var configService = new ConfigurationDataService();
            var botTimerService = new BotTimerDataServiceProvider();
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            await botTimerService.WriteTimestampToTextAsync();

            var dBuilder = DiscordClientBuilder.CreateDefault(token.Value, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.All);

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
                services.AddScoped<IGuildMemberService, GuildMemberService>();
                services.AddScoped<IBotTimer, BotTimerDataServiceProvider>();
            });
            #endregion


            dBuilder.UseInteractivity();
            
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
                        if (e.Message!.Author!.Id.Equals(1076279184667721859))
                        {
                            if (e.Guild.Id.Equals(764184337620140062) && e.Message!.Channel!.Id.Equals(1076279102841045093))
                            {
                                var channel = await s.GetChannelAsync(1367596778362241086); 
                                await channel.SendMessageAsync("reload");
                            }
                        }

                        if (e.Message.Author!.IsBot && e.Message.Content.Equals("reload"))
                        {
                            await e.Message.RespondAsync("``documentation reloaded``");
                            Console.WriteLine(
                                $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkYellow("Documents Reloaded!")}");
                            return;
                        }
                        
                        if (e.Message.Content.Contains("help"))
                        {
                            var user = e.Author;
                            await e.Channel.SendMessageAsync(
                                "I noticed you needed some help.....please use the ``/help`` command to see a list of help topics");
                            return;
                        }
                    })
                    
                    #region CHANNEL CREATED
                    .HandleChannelCreated(async (s, e) =>
                    {

                    })
                    #endregion

                    #region DOWNLOAD COMPLETE
                    .HandleGuildDownloadCompleted(async (e, s) =>
                    {
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Guild Download Complete.")}");
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Connection Success, Listening for events...")}");
                        timerService.CreateNew();
                        timerService.Start();
                    })

                   #endregion

                    #region SESSION CREATED
                    .HandleSessionCreated(async (s,e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"[{DateTimeOffset.UtcNow}] [Gameday Tracker] ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($"[INFO] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("shaking hands with discord...");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"[{DateTimeOffset.UtcNow}] [Gameday Tracker] ");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($"[INFO] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Session Started!");
                        Console.WriteLine();
                        Console.ResetColor();
                    })
                   #endregion

                    #region GUILD CREATED
                    .HandleGuildCreated(async (s, e) =>
                    {
                        await using var db = new BotDbContextFactory().CreateDbContext();
                        var guild = configService.GuildExists(e.Guild);
                        if (!guild.IsOk)
                        {
                            var g = new Guild()
                            {
                                DateAdded = DateTimeOffset.UtcNow,
                                GuildId = (long)e.Guild.Id,
                                GuildName = e.Guild.Name,
                                GuildOwnerId = (long)e.Guild.OwnerId,
                                NotificationChannelId = (long)e.Guild.SystemChannelId!
                            };
                            db.Guilds.Add(g);
                            await db.SaveChangesAsync();
                        }

                    })
                   #endregion

                );


            #endregion

            
            var status = new DiscordActivity("Game-Day", DiscordActivityType.Watching);
            var client = dBuilder.Build();
            dBuilder.SetReconnectOnFatalGatewayErrors();
            await client.ConnectAsync(status, DiscordUserStatus.Online, DateTimeOffset.UtcNow);
           
            await Task.Delay(-1);
        }

    }
}
