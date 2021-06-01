using Discord.Commands;
using Discord;
using ISurvivalBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISurvivalBot.Utils;

namespace ISurvivalBot.Commands
{


    [Group("user")]
    public class UserCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandCountService _commandCountService;
        private readonly UserService _userService;


        public UserCommand(CommandCountService commandCountService, UserService userService)
        {
            this._userService = userService;
            this._commandCountService = commandCountService;
        }


        [Command("countword")]
        public async Task CountWord(string username, string commandReq)
        {
            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();
            string result = "";
            if (mentionUser != null)
            {
                result = await _commandCountService.GetCounterByUser(mentionUser.Username, commandReq);
            }
            else
            {
                result = await _commandCountService.GetCounterByUser(username, commandReq);
            }
            await Context.Message.ReplyAsync(result);

        }


        [Command("isadmin", RunMode = RunMode.Async)]
        public async Task IsAdminCommand(string username)
        {
            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();
            bool result =  await _userService.IsAdmin(mentionUser == null ? username : mentionUser.Username);
            await Context.Message.AddReactionAsync(result ? CommonEmoij.OK : CommonEmoij.NOK);
        }


        [Command("setadmin", RunMode = RunMode.Async)]
        public async Task CreateAdminCommand(IUser user, bool admin)
        {
            bool isRequesterAdmin = await _userService.IsAdmin(Context.Message.Author.Username);
            if (!isRequesterAdmin)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }

            var result = await _userService.SetAdminStatus(user.Username, admin);
            await Context.Message.AddReactionAsync(result ? CommonEmoij.OK : CommonEmoij.NOK);
        }

        [Command("resetwordcount")]
        public async Task ResetWordCount(IUser user, string word)
        {
            bool isRequesterAdmin = await _userService.IsAdmin(Context.Message.Author.Username);
            if (!isRequesterAdmin)
            {
                await Context.Message.AddReactionAsync(CommonEmoij.NOK);
                return;
            }

            var result = await _commandCountService.ResetWordCount((long)user.Id, word);
            await Context.Message.AddReactionAsync(result ? CommonEmoij.OK : CommonEmoij.NOK);
        }

    }
}
