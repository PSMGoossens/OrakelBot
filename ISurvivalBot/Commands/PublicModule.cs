using Discord;
using Discord.Commands;
using ISurvivalBot.Services;
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

        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            await Context.Message.ReplyAsync("pong!");
        }


        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("catfact")]
        public async Task CatFactAsync()
        {
            // Get random cat fact
            var result = await PictureService.GetCatFact();
            await Context.Message.ReplyAsync(result);
        }

        [Command("terry")]
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
        public async Task DogAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetDogPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "dog.png");
        }

        [Command("fox")]
        public async Task FoxAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetFoxPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "fox.png");
        }

        [Command("panda")]
        public async Task PandaAsync()
        {
            // Get a stream containing an image of a dog
            var stream = await PictureService.GetPandaPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "panda.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }


        [Command("howto")]
        public async Task HowTocommand(string question)
        {
            question = HttpUtility.UrlEncode(question);
            await ReplyAsync($"https://letmegooglethat.com/?q={question}");
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
