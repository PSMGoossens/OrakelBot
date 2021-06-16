using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Models
{
    public class BotContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<CommandCount> CommandCount { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        private ILoggerFactory _loggingFactory;

        public DbSet<User> Users { get; set; }


        public BotContext(ILoggerFactory loggerFactory)
        {
            _loggingFactory = loggerFactory;
        }
        //public DbSet<CommandCounter> CommandCounters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            string path = Path.Combine(Environment.CurrentDirectory, "bot.db");
            options.UseSqlite($"Data Source = {path}");
            //options.UseLoggerFactory(_loggingFactory);
        }

        internal void EnqureCreated()
        {
            /*foreach (var q in Quotes)
            {
                this.Quotes.Remove(q);
            }
            SaveChanges();
            this.Database.EnsureCreated();*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>()
                .HasKey(q => q.Id);
        }

        /*public async Task<int> SheepCommandCount()
        {
            //var count = await CommandCount.Where(cc => cc.Command.Equals("")).AsAsyncEnumerable().ToList();

            return count.;
        }*/
    }
}
