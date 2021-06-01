using ISurvivalBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class CommandCountService
    {

        private readonly BotContext _botContext;


        public CommandCountService(BotContext botContext)
        {
            this._botContext = botContext;
        }




        public async Task<int> CountAndIncrementCommandByUser(string command, long userId, int wordCount = 1)
        {
            int count = 0;
            var commandItem = await _botContext.CommandCount.AsAsyncEnumerable().Where(cc => cc.Command == command && cc.User.DiscordId == userId).FirstOrDefaultAsync();
            if (commandItem == null)
            {
                User user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.DiscordId == userId).FirstAsync();
                CommandCount cc = new ()
                {
                    Command = command,
                    Count = wordCount,
                    User = user
                };

                await _botContext.CommandCount.AddAsync(cc);
                await _botContext.SaveChangesAsync();
                return 1;
            }
            else
            {
                commandItem.Count+= wordCount;
                count = commandItem.Count;
                _botContext.Update(commandItem);
                await _botContext.SaveChangesAsync();
            }

            return count;

        }


        public async Task<string> GetCounterByUser(string username, string command)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.CurrentUsername == username).FirstOrDefaultAsync();
            if (user == null)
                return "Invalid user";

            var countCommand = await _botContext.CommandCount.AsAsyncEnumerable().Where(cc => cc.Command == command && cc.User == user).FirstOrDefaultAsync();
            if (countCommand == null)
                return  $"{username}: heeft nog nooit het woord {command} gebruikt.";

            return $"{username}: heeft {countCommand.Count} keer het woord {command} gebruikt.";
        }

        public async Task<bool> ResetWordCount(long id, string command)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.DiscordId == id).FirstOrDefaultAsync();
            if (user == null)
                return false;

            var countCommand = await _botContext.CommandCount.AsAsyncEnumerable().Where(cc => cc.Command == command && cc.User == user).FirstOrDefaultAsync();
            if (countCommand == null)
                return false;

            countCommand.Count = 0;
            _botContext.CommandCount.Update(countCommand);
            await _botContext.SaveChangesAsync();
            return true;

        }
    }
}
