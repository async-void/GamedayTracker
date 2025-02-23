using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Services;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Interactivity.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamedayTracker
{
    public class Bot
    {
        private readonly TimerService timerService = new TimerService();

        public async Task RunAsync()
        {
            
            var configService = new ConfigurationDataService();
            var interactionService = new InteractionDataProviderService();
            
            
            var token = configService.GetBotToken();
            var prefix = configService.GetBotPrefix();

            var dBuilder = DiscordClientBuilder.CreateDefault(token.Value, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.All);
            dBuilder.UseInteractivity();
            
            dBuilder.UseCommands ((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands(Assembly.GetExecutingAssembly());

                TextCommandProcessor textCommandProcessor = new(new()
                {
                    PrefixResolver = new DefaultPrefixResolver(true, prefix.Value).ResolvePrefixAsync,
                });

                extension.AddProcessor(textCommandProcessor);
            }, new CommandsConfiguration()
            {
                RegisterDefaultCommandProcessors = true,
                DebugGuildId = 0,

            });

            #region EVENT HANDLERS

            dBuilder.ConfigureEventHandlers(
                m => m.HandleMessageCreated(async (s, e) =>
                    {
                        if (e.Message.Author!.IsBot) return;
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

                        //var teamData = new TeamDataService();
                        //var nubs = await teamData.GetDraftResultsAsync(2024);
                    })

                   #endregion

                    #region SESSION CREATED
                    .HandleSessionCreated(async (s, e) =>
                    {
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("shaking hands with discord...")}");
                        Console.WriteLine(
                            $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray("Session Started!")}");
                    })
                #endregion

                    #region INTERACTION CREATED
                    .HandleComponentInteractionCreated(async (client, args) =>
                    {
                        await args.Interaction.DeferAsync();
                        //var buttonContent = "";
                        //var buttonContentResult = interactionService.ParseButtonId(args.Id);
                        if (args.Id == "nextObj")
                        {
                            var message = new DiscordMessageBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()
                                    .WithTitle("Not Implemented Yet!")
                                    .WithDescription(
                                        "add player picks is not yet implemented. the bot devs are hard at work with the next update.")
                                    .WithTimestamp(DateTime.UtcNow));
                            await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(message));



                        }
                        //buttonContent = buttonContentResult.IsOk ? buttonContentResult.Value : buttonContentResult.Error.ErrorMessage;

                        //await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder()
                        //    .WithContent($"Button {buttonContent} Clicked"));
                    })
                #endregion

                    #region GUILD CREATED
                    .HandleGuildCreated(async (s, e) =>
                    {
                        await using var db = new BotDbContextFactory().CreateDbContext();
                        var guild = configService.GuildExists(e.Guild);
                        if (guild.IsOk)
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

            #region CONFIGURE SERVICES
            dBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<ITeamData, TeamDataService>();
                services.AddScoped<ITimerService, TimerService>();
                services.AddScoped<ILogger, LoggerService>();
                services.AddScoped<IConfigurationData, ConfigurationDataService>();
            });
            #endregion

            var status = new DiscordActivity("Game-Day", DiscordActivityType.Watching);
            var client = dBuilder.Build();

            dBuilder.SetReconnectOnFatalGatewayErrors();
            await client.ConnectAsync(status, DiscordUserStatus.Online, DateTimeOffset.UtcNow);
           
            await Task.Delay(-1);
        }
    }
}
