using ISurvivalBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class UserService
    {

        private readonly BotContext _botContext;


        public UserService(BotContext botContext)
        {
            this._botContext = botContext;
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
            

        }

        public async Task UpdateUsers(List<Tuple<long, string>> userList)
        {
            foreach (var user in userList)
            {
                await UpdateUser(user.Item1, user.Item2);
            }
            await _botContext.SaveChangesAsync();

        }

        public async Task<bool> SetAdminStatus(string username, bool admin)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.CurrentUsername == username).FirstOrDefaultAsync();
            if (user == null)
                return false;

            user.IsAdmin = admin;
            _botContext.Users.Update(user);
            await _botContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsAdmin(string username)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.CurrentUsername == username).FirstOrDefaultAsync();
            if (user == null)
                return false;
            return user.IsAdmin;
        }

        public async Task<bool> IsAdmin(long userId)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.DiscordId == userId).FirstOrDefaultAsync();
            if (user == null)
                return false;
            return user.IsAdmin;
        }

        public async Task<ulong>  GetDiscordIdByUsername(string username)
        {
            var user = await _botContext.Users.AsAsyncEnumerable().Where(u => u.CurrentUsername == username).FirstOrDefaultAsync();
            if (user == null)
                return 0;
            else
                return (ulong)user.DiscordId;
        }
    }
}
