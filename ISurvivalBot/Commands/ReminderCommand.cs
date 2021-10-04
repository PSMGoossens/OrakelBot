using Discord;
using Discord.Commands;
using ISurvivalBot.Models;
using ISurvivalBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Commands
{
    [Group("reminder")]
    public class ReminderCommand : ModuleBase<SocketCommandContext>
    {

        private readonly ReminderService _reminderService;
        private readonly UserService _userService;

        public ReminderCommand(ReminderService reminderService, UserService userService)
        {
            _reminderService = reminderService;
            _userService = userService;
        }


        [Command("add", RunMode = RunMode.Async)]
        public async Task AddReminderCommand(IUser user, string chronos, string text)
        {
            var userAccount = await _userService.GetUserByUsername((long)user.Id);
            var result = await _reminderService.Add(new Reminder
            {
                CronosFormat = chronos,
                ForUser = userAccount,
                Text = text
            });
        }

        [Command("del", RunMode = RunMode.Async)]
        public async Task DelReminderCommand(long reminderId)
        {
            var result = await _reminderService.Delete(reminderId);
        }

        [Command("list", RunMode = RunMode.Async)]
        public async Task ListReminderCommand(IUser user = null)
        {

        }
    }
}
