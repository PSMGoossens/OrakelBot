using Discord;
using Discord.Commands;
using ISurvivalBot.Services;
using ISurvivalBot.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ISurvivalBot.Commands
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }
        private readonly static Random random = new Random();

        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            await Context.Message.ReplyAsync("pong!");
        }

        [Command("help")]
        [Summary("Help command")]
        public async Task HelpCommand()
        {
            await Context.Message.ReplyAsync("Even geduld a.u.b. de bot met een helpfunctie is ook binnenkort op uw discord server beschikbaar!");
        }


        [Command("cat")]
        [Summary("Shows a picture of a random cat.")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("catfact")]
        [Summary("Reacts with a random cat fact.")]
        public async Task CatFactAsync()
        {
            // Get random cat fact
            var result = await PictureService.GetCatFact();
            await Context.Message.ReplyAsync(result);
        }

        [Command("terry")]
        [Summary("TempleOS")]
        public async Task TerryCommand()
        {
            Emoji custom = null;
            foreach(var emoij in Context.Guild.Emotes)
            {
                Console.WriteLine($"Name: {emoij.Name} Url: {emoij.Url}, Id: {emoij.Id}");
            }
            await Context.Message.AddReactionAsync(Emote.Parse("<:templeos:814952144851042364>"));
        }


        [Command("dog")]
        [Summary("Shows a picture of a random dog.")]
        public async Task DogAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetDogPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "dog.png");
        }

        [Command("fox")]
        [Summary("Shows a picture of a random fox.")]
        public async Task FoxAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetFoxPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "fox.png");
        }

        [Command("panda")]
        [Summary("Shows a picture of a random panda.")]
        public async Task PandaAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetPandaPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "panda.png");
        }


        [Command("howto")]
        public async Task HowTocommand(string question)
        {
            question = HttpUtility.UrlEncode(question);
            await ReplyAsync($"https://letmegooglethat.com/?q={question}");
        }

        [Command("berend")]
        public async Task BerendCommand()
        {
            await Context.Message.AddReactionAsync(CommonEmoij.BEREND);
        }

        [Command("kopmunt")]
        public async Task KopMuntCommand()
        {
            await ReplyAsync(random.Next(0, 1) == 1 ? "Kop" : "Munt");
        }

        // Ban a user
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have the permissions to ban a user")]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I don't have the permissions to ban a user")]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

    }
}
