using Discord.Commands;
using ISurvivalBot.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ISurvivalBot.Commands
{
    public class SheepCommand : ModuleBase<SocketCommandContext>
    {

        private readonly BotContext _botContext;
        //private static uint SheepCount = 0;


        public SheepCommand(BotContext botContext)
        {
            this._botContext = botContext;
        }

        [Command("meh")]
        public async Task MehCommand()
        {
            int count = await SheepCounter((long)Context.User.Id);
            await ReplyAsync($"{Context.User.Username} heeft {count} schaapjes geteld");
        }


        public async Task<int> SheepCounter(long userId)
        {
            int count = 0;
            var commandItem = await _botContext.CommandCount.AsAsyncEnumerable().Where(cc => cc.Command == "meh" && cc.User.DiscordId == userId).FirstOrDefaultAsync();
            if (commandItem == null)
            {
                User user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.DiscordId == userId).FirstAsync();
                CommandCount cc = new CommandCount
                {
                    Command = "meh",
                    Count = 1,
                    User = user
                };

                await _botContext.CommandCount.AddAsync(cc);
                await _botContext.SaveChangesAsync();
                return 1;
            }
            else
            {
                

                commandItem.Count++;
                count = commandItem.Count;
                _botContext.Update(commandItem);
                await _botContext.SaveChangesAsync();
            }

            return count;

        }
    }
}
