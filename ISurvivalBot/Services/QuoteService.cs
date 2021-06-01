using ISurvivalBot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class QuoteService
    {

       // private readonly ILogger _logger;
        private readonly BotContext _botContext; 
        private Random rand = new Random();

        public QuoteService(/*ILogger logger,*/ BotContext botContext)
        {
            //this._logger = logger;
            this._botContext = botContext;
        }


        public async Task<string> RandomQuote()
        {
            int quoteCount = _botContext.Quotes.Count();
            if (quoteCount == 0)
                return "Error: No quotes in database.";
            int toSkip = rand.Next(1, quoteCount);
            Quote quote = await _botContext.Quotes.AsAsyncEnumerable().OrderBy(r => Guid.NewGuid()).Skip(toSkip).Take(1).FirstAsync();

            return $"{quote.QuoteAboutUser}: {quote.QuoteText}";
        }

        public async Task AddQuote(string userAdded, string unparsedText)
        {
            unparsedText = unparsedText.Replace("!quote add", "").Trim();
            int indexOfDoublePoint = unparsedText.IndexOf(":");
            if (indexOfDoublePoint < 0)
                return;
            string aboutUser = unparsedText.Substring(0, indexOfDoublePoint);
            string quoteText = unparsedText.Substring(indexOfDoublePoint+1, unparsedText.Length- indexOfDoublePoint-1);
            Quote quote = new Quote
            {
                AddedByUser = userAdded,
                AddedAt = DateTime.Now,
                QuoteText = quoteText,
                QuoteAboutUser = aboutUser,
            };

            await _botContext.Quotes.AddAsync(quote);
            _botContext.SaveChanges();
        }

        public async Task<int> QuoteCountUser(string user)
        {
            return await _botContext.Quotes.AsAsyncEnumerable().WhereAwait(c => new ValueTask<bool>(c.QuoteAboutUser == user)).CountAsync();
        }

        public async Task<int> QuoteCount()
        {
            return await _botContext.Quotes.CountAsync();
        }
    }
}
