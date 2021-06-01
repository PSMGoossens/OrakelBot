using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class StateHandlerService
    {

        public IAudioClient AudioClient { get; set; }

        /*public StateHandlerService(IAudioClient audioClient)
        {
            this.AudioClient = audioClient;
        }*/
    }
}
