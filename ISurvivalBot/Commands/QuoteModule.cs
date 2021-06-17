using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ISurvivalBot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Commands
{


    [Group("quote")]
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {


        private readonly QuoteService _quoteService;

        //private readonly ILogger _logger;


        public QuoteModule(/*ILogger logger,*/ QuoteService quoteService) {
            //_logger = logger;
            _quoteService = quoteService;
        }



        [Command("add", RunMode = RunMode.Async)]
        public async Task AddQuoteCommand(string username, string quote)
        {
            var mentionUser = Context.Message.MentionedUsers.FirstOrDefault();
            // No user at the server
            if (mentionUser == null)
            {

            }
            await _quoteService.AddQuote(Context.User.Username, Context.Message.Content);
        }



        [Command("random", RunMode = RunMode.Async)]
        public async Task RandomQuoteCommand()
        {
            var quote = await _quoteService.RandomQuote();
            await Context.Message.ReplyAsync(quote);
        }

        [Command("count", RunMode = RunMode.Async)]
        public async Task QuoteCountCommand()
        {
            var quoteCount = await _quoteService.QuoteCount();
            await Context.Message.ReplyAsync($"Number of quotes in database {quoteCount}");
        }

        [Command("usercount", RunMode = RunMode.Async)]
        public async Task QuoteCountUsercommand(string user)
        {
            var quoteCount = await _quoteService.QuoteCountUser(user);
            await Context.Message.ReplyAsync($"Number of quotes by user {user}: {quoteCount}");
        }



    }
}
