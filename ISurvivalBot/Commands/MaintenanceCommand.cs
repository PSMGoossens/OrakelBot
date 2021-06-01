using Discord;
using Discord.Commands;
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

        public MaintenanceCommand(MaintenanceService maintenanceservice, UserService userService)
        {
            this._maintenanceService = maintenanceservice;
            this._userService = userService;
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
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);

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


    }
}
