using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Commands
{
    public class JamaarCommand : ModuleBase<SocketCommandContext>
    {

        private readonly ILogger<JamaarCommand> _logger;

        public JamaarCommand(ILogger<JamaarCommand> logger) => _logger = logger;



        public static Random random = new Random();
        public List<String> cards = new List<string>()
        {
            "Nee, het zal je niet lukken",
            "Ja, maar het zal niet makkelijk gaan",
            "Ja, maar wees voorbereid op tegenslag",
            "Ja, waag het",
            "Ja, maar hou zelf het initiatief",
            "Ja, het is het begin van een groots avontuur",
            "Ja, maar alleen als je je kunt openstellen om te ontvangen",
            "Nee, het zal je ongelukkig maken",
            "Ja, maar blijf luisteren naar je lichaam",
            "Nee, het is de moeite niet waard",
            "Nee, je zal van jezelf vervreemden",
            "Nee, het is niet je diepste verlangen",
            "Ja, vecht ervoor",
            "Ja, het zal je genezen",
            "Nee, het gaat ten koste van je gezondheid",
            "Ja, maar stop met zeuren en piekeren",
            "Ja, zelfs als alle voortekenen ongunstig zijn",
            "Ja, maar doe het tactvol",
            "Ja, het zal je diepgaand veranderen",
            "Ja, maar alleen als je je kunt verweren",
            "Ja, maar je moet bereid zijn het risico echt te nemen",
            "Nee, en trek je terug uit deze situatie",
            "Nee, je doet het om je te conformeren",
            "Nee, je motitieven zij niet zuiver",
            "Nee, het is ondoordracht",
            "Nee, het zal in alle opzichten tegenvallen",
            "Nee, vergeet het",
            "Nee en stop met zoeken naar wat je eigenlijk al hebt",
            "Nee, het past niet bij je",
            "Ja, maar vergeet niet dat het om de weg gaat, niet om de bestemming",
            "Nee, het is een vlucht",
            "Nee, wacht op een betere kans",
            "Ja, het betekent een nieuwe en frisse start",
            "Ja, maar alleen als je niet in goed en kwaad denkt",
            "Ja, aanvaard het als een kostbaar geschenk",
            "Ja, beslist!",
            "Nee, het zal je teveel energie kosten",
            "Ja en het zal het beste in jezelf naar boven halen",
            "Nee, het zal je in gevaar brengen",
            "Ja, maar verbrand je schepen achter je niet",
            "Ja, de einige die twijfelt ben jij",
            "Ja, je opent een geweldige energiebron",
            "Ja, het zal zelfs makkelijker gaan dan je denkt",
            "Ja en geniet er van",
            "Ja, maar doe het met een glimlach",
            "Ja, het is tijd voor een verandering",
            "Ja, je bent er klaar voor",
            "Ja, het zal je rust geven",
            "Ja, het mag",
            "Ja, dit is een once-in-a-lifetime mogelijkheid",
            "Ja, maar blijf je hart volgen",
            "Ja, maar accepteer dat je leven grondig zal veranderen",
            "Nee, dit is niet het moment",
            "Ja, maar let op de details",
            "Ja, het is precies de goede stap op precies het goede moment",
            "Ja, maar alleen als je geduld hebt",
            "Ja, je bent het waard",
            "Ja, maar met mededogen",
            "Ja, maar alleen als het open & eerlijk kan",
            "Nee, je verdient beter",
            "Nee, doe eerst iets anders wat je op dit moment liever wilt",
            "Ja, maar alleen als je het conflict niet uit de weg gaat",
            "Nee, het is te hoog gegrepen",
            "Nee, het is een illusie",
            "Ja, maar alleen als je echt op succes bent voorbereid",
            "Ja, maar loop niet te hard van stapel",
            "Ja, dit is het moment",
            "Ja en laat het los",
            "Ja, maar accepteer onoplosbare conflicten",
            "Ja, je zal er veel van leren",
            "Ja, maar alleen als je je eigenbelang opzij kunt zetten",
            "Ja, je bloed zal er sneller van stromen",
            "Ja, maar houd afstand",
            "Nee, je zal er onder bezwijken",
            "Nee, zelfs als alle voortekenen gunstig zijn",
            "Ja, het zal je gelukkig maken",
            "Nee, je zal je vrijheid verliezen",
            "Ja, maar vraag hulp"

        };



        [Command("jamaar")]
        [Summary("Stel een dilemma voor aan de bot. Dit geef je achter !jamaar in de tekst aan")]
        public async Task Jamaar([Remainder] string text)
        {
            int answer = random.Next(this.cards.Count);
            _logger.LogInformation($"Jamaar Command from: {Context.Message.Author.Username} on channel {Context.Channel.Name}, Question: \"{text}\" Answer: {this.cards[answer]}.");
            await Context.Message.ReplyAsync(this.cards[answer]);
        }
    }
}
