using Cronos;
using Discord.WebSocket;
using Discord;
using ISurvivalBot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;

namespace ISurvivalBot.Services
{

    internal class ReminderConverted
    {
        public Reminder Reminder { get; set; }
        public CronExpression Expression { get; }

        public DateTime? NextTime { get; private set; }

        public SocketUser User { get; private set; }


        public ReminderConverted(Reminder reminder, SocketUser socketUser)
        {
            this.Reminder = reminder;
            this.User = socketUser;
            this.Expression = CronExpression.Parse(reminder.CronosFormat);
            Update();
        }

        public void Update()
        {
            TimeZoneInfo localTmeZone = TimeZoneInfo.Local;
            this.NextTime = Expression.GetNextOccurrence(DateTime.UtcNow, localTmeZone);

        }


    }
    public class ReminderService : IDisposable
    {
        private readonly ILogger<ReminderService> _logger;
        private readonly BotContext _botContext;
        private ReminderConverted nextEvent;
        private Timer timerTask;
        private readonly UserService _userService;
        private readonly DiscordSocketClient _discordSocketClient;


        private ConcurrentDictionary<long, ReminderConverted> reminders = new ConcurrentDictionary<long, ReminderConverted>();

        public ReminderService(ILogger<ReminderService> logger, BotContext botContext, UserService userService, DiscordSocketClient discordSocketClient)
        {
            _logger = logger;
            _botContext = botContext;
            this._userService = userService;
            _discordSocketClient = discordSocketClient;
            Init();
        }

        public void Init()
        {
            var user = _userService.GetDiscordUser(438976181237448705);
            nextEvent = new ReminderConverted(new Reminder
            {
                Id = 0,
                ForUser = new User
                {
                    DiscordId = 438976181237448705,
                    CurrentUsername = "Cameron90"
                },
                CronosFormat = "*/2 * * * *",
                Text = "Dit is een test bericht"
            }, user);

            // Timer that checks every minute if the task needs to be executed
            timerTask = new Timer(1000 * 60);
            timerTask.Elapsed += TimerTask_Elapsed;
            timerTask.Start();
        }


        public void Dispose()
        {
            timerTask.Stop();
        }

        private void TimerTask_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeDiff = nextEvent.NextTime.Value.Subtract(DateTime.UtcNow);
            if (timeDiff.TotalMinutes > -1 && timeDiff.TotalMinutes < 0)
            {
                // Execute task here?
                var user = _userService.GetDiscordUser((ulong)nextEvent.Reminder.ForUser.DiscordId);
                user.SendMessageAsync($"Om {nextEvent.NextTime.Value.ToShortTimeString()} denk dan aan {nextEvent.Reminder.Text}.");
                //_discordSocketClient.send
            }
            nextEvent.Update();
        }

        public void DownloadFromDatabase()
        {
            //TaskScheduler.FromCurrentSynchronizationContext()

        }

        public void Run()
        {
            
        }


        public void Update()
        {

        }

        public async Task<bool> Add(Reminder reminder)
        {
            // Check for valid chrono
            try
            {
                var result = CronExpression.Parse(reminder.CronosFormat);
            } catch (CronFormatException cfe) {
                return false;
            }


            // Add towards the database 
            await _botContext.Reminders.AddAsync(reminder);
            await _botContext.SaveChangesAsync();

            // Update the current list
            var user = _userService.GetDiscordUser((ulong)reminder.ForUser.DiscordId);
            ReminderConverted reminderConverted = new ReminderConverted(reminder, user);

            return true;

        }

        public async Task<bool> Delete(long reminderId)
        {
            var result = await _botContext.Reminders.AsAsyncEnumerable().Where(r => r.Id == reminderId).FirstOrDefaultAsync();
            if (result == null)
                return false;

            // Remove from current list
            if (reminders.ContainsKey(reminderId))
            {
                // Delete from the dictonairy
                ReminderConverted output;
                if (!reminders.TryRemove(reminderId, out output))
                {
                    _logger.LogWarning($"Cannot delete reminder from internal queue with id {reminderId}.");
                }

            }

            _botContext.Reminders.Remove(result);
            await _botContext.SaveChangesAsync();

            return true;
        }




        private void ExecuteTask()
        {
            // Run task for now

            // Set the newest Task
        }

    }
}
