using ISurvivalBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class MaintenanceService
    {
        public readonly BotContext _botContext;


        public MaintenanceService(BotContext botContext)
        {
            this._botContext = botContext;
        }


        public async Task UpdateUsers(List<Tuple<ulong, string>> users) 
        {

        }

        public async Task UpdateUser(long userId, string username)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.DiscordId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                User u = new User
                {
                    DiscordId = userId,
                    CurrentUsername = username,
                };
                await _botContext.Users.AddAsync(u);
            }
            else if (user.CurrentUsername != username)
            {
                user.CurrentUsername = username;
                _botContext.Users.Update(user);
            }
            await _botContext.SaveChangesAsync();


        }
    }
}
