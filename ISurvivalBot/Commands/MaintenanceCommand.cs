using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ISurvivalBot.Services;
using ISurvivalBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Commands
{


    [Group("maintenance")]
    public class MaintenanceCommand : ModuleBase<SocketCommandContext>
    {

        private readonly MaintenanceService _maintenanceService;
        private readonly UserService _userService;
        private readonly DiscordSocketClient _discordSocketClient;

        public MaintenanceCommand(MaintenanceService maintenanceservice, UserService userService, DiscordSocketClient discordSocketClient)
        {
            this._maintenanceService = maintenanceservice;
            this._userService = userService;
            this._discordSocketClient = discordSocketClient;
        }


        [Command("update", RunMode = RunMode.Async)]
        public async Task UpdateCommand()
        {
            await _maintenanceService.UpdateUser((long)Context.User.Id, Context.User.Username);
        }


        [Command("setadmin", RunMode = RunMode.Async)]
        public async Task CreateAdminCommand(string username, bool admin)
        {
            bool isRequesterAdmin = await _userService.IsAdmin(Context.Message.Author.Username);
            if (!isRequesterAdmin)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }

            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();
            if (mentionUser != null)
                username = mentionUser.Username;

            var result = await _userService.SetAdminStatus(username, admin);
            await Context.Message.AddReactionAsync(result ? CommonEmoij.OK : CommonEmoij.NOK);
        }


        [Command("isadmin", RunMode = RunMode.Async)]
        public async Task IsAdminCommand(string username)
        {
            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();
            bool result = await _userService.IsAdmin(mentionUser == null ? username : mentionUser.Username);
            await Context.Message.AddReactionAsync(result ? CommonEmoij.OK : CommonEmoij.NOK);

        }

        [Command("sayprivate", RunMode = RunMode.Async)]
        public async Task SayPrivate(ulong userId, string text)
        {
            bool isRequesterAdmin = await _userService.IsAdmin(Context.Message.Author.Username);
            if (!isRequesterAdmin)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }

            var userSend = _discordSocketClient.GetUser(userId);
            if (userSend == null)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            } 
            else
            {
                await userSend.SendMessageAsync(text);
                await Context.Message.AddReactionAsync(CommonEmoij.OK);
                return;
            }
        }

        [Command("sayprivate", RunMode = RunMode.Async)]
        public async Task SayPrivate(string userName, string text)
        {
            bool isRequesterAdmin = await _userService.IsAdmin(Context.Message.Author.Username);
            if (!isRequesterAdmin)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }
            var discordUserId = await _userService.GetDiscordIdByUsername(userName);
            var userSend = _discordSocketClient.GetUser(discordUserId);
            if (userSend == null)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }
            else
            {
                await userSend.SendMessageAsync(text);
                await Context.Message.AddReactionAsync(CommonEmoij.OK);
                return;
            }
        }


    }
}
