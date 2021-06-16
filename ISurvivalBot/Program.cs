using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ISurvivalBot.Models;
using ISurvivalBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISurvivalBot
{
    class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private ILogger _logger;
        private ServiceProvider services;

        public Program()
        {
            // create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }


        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using ( services = ConfigureServices())
            {


                //services.GetService<ILoggerFactory>().AddProvider(new )
                _logger = services.GetService<ILoggerFactory>().CreateLogger<Program>();
                _logger.LogInformation("Example log message");
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                client.GuildAvailable += Client_GuildAvailable;

                // setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                client.MessageReceived += Client_MessageReceived;
                client.GuildMemberUpdated += Client_GuildMemberUpdated;
                client.UserUpdated += Client_UserUpdated;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                client.GuildMembersDownloaded += Client_GuildMembersDownloaded;

                await client.DownloadUsersAsync(client.Guilds);


                //clien

                // client.DownloadUsersAsync()

                //await 

                // Update user database
                //await updateUsers(client);

                await Task.Delay(-1);
            }
        }

        private async Task Client_GuildMembersDownloaded(SocketGuild guild)
        {
            _logger.LogInformation($"Downloaded Guild {guild.Name} becomes available, with id: {guild.Id}");

            var userList = new List<Tuple<long, string>>();

            foreach (var user in guild.Users)
            {
                userList.Add(Tuple.Create((long)user.Id, user.Username));
            }

            await services.GetRequiredService<UserService>().UpdateUsers(userList);

        }

        private async Task Client_GuildAvailable(SocketGuild guild)
        {
            _logger.LogInformation($"Guild {guild.Name} becomes available, with id: {guild.Id}");
        }

        private async Task updateUsers(DiscordSocketClient client)
        {

            var channels = client.Guilds;
            // Check if channels are available
            if (channels.Count() == 0)
                return;

            // Get the users from the first channel
            var channel = channels.First();
            await foreach(var lst in channel.GetUsersAsync())
            {
                foreach (var user in lst)
                {
                    _logger.LogInformation($"User id: {user.Id}, Username: {user.Username}");
                }
            }
        }

        private async Task Client_UserUpdated(SocketUser old, SocketUser newU)
        {
            //throw new NotImplementedException();
            _logger.LogInformation($"User {old.Username} with status {old.Status} becomes user: {newU.Username} new status {newU.Status}");
        }

        private  async Task Client_GuildMemberUpdated(SocketGuildUser old, SocketGuildUser newU)
        {
            //throw new NotImplementedException();
            //_logger.LogInformation($"Old user: {old.Username} new user: {newU.Username}, Status: {newU.Status}");
            _logger.LogInformation($"User {old.Username} with status {old.Status} becomes user: {newU.Username} new status {newU.Status}");
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {

            string messageText = message.Content.ToLowerInvariant();
            if (message.Author.Username == "Het Orakel" || message.Content.StartsWith("!"))
                return;
            if (messageText.Contains("bruh"))
            {
                int wordCount = wordDuplication(messageText, "bruh");
                var commandCountService = services.GetRequiredService<CommandCountService>();
                var counter = await commandCountService.CountAndIncrementCommandByUser("bruh", (long)message.Author.Id, wordCount);
                await message.Channel.SendMessageAsync($"{message.Author.Username} heeft {counter} keer bruh gezegd!");
            }
            else if (messageText.Contains("meh"))
            {
                int wordCount = wordDuplication(messageText, "meh");
                var commandCountService = services.GetRequiredService<CommandCountService>();
                var counter = await commandCountService.CountAndIncrementCommandByUser("meh", (long)message.Author.Id, wordCount);
                await message.Channel.SendMessageAsync($"{message.Author.Username} heeft {counter} schaapjes geteld!");
            }
        }

        private int wordDuplication(string text, string word)
        {
            string[] words = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> statistics = words
                .GroupBy(word => word)
                .ToDictionary(
                     kvp => kvp.Key, // the word itself is the key
                    kvp => kvp.Count()); // number of occurences is the value
            if (statistics.ContainsKey(word))
                return statistics[word]; 
            return 1;
        }

        private Task LogAsync(LogMessage log)
        {
            _logger.LogInformation(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            _logger.LogInformation($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddLogging(opt =>
                {
                    opt.AddConsole();
                    //opt.AddFilter((category, level) =>
                    //    category == DbLoggerCategory.Database.Command.Name
                    //    && level == LogLevel.Information);
                })
                .AddSingleton(_config)
                .AddSingleton<BotContext>()
                //.AddSingleton<DiscordSocketClient>()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<StateHandlerService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<AudioService>()
                .AddSingleton<QuoteService>()
                .AddSingleton<CommandCountService>()
                .AddSingleton<MaintenanceService>()
                .AddSingleton<UserService>()
                .BuildServiceProvider();
        }
    }
}
