using Discord;
using Discord.Commands;
using ISurvivalBot.Services;
using ISurvivalBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISurvivalBot.Commands
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot
        public AudioModule(AudioService service)
        {
            _service = service;
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("joins", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild, ErrorMessage = "This command is only possible via a public channel.")]
        //[RequireBotPermission(ChannelPermission.Speak, ErrorMessage = "I don't have permissions to join a voice chat")]
        public async Task JoinCmd()
        {
            var result = await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
            
            /*Emoji custom = null;
            foreach(var emoij in Context.Guild.Emotes)
            {
                Console.WriteLine($"Name: {emoij.Name} Url: {emoij.Url}, Id: {emoij.Id}");
            }*/
            await Context.Message.AddReactionAsync(CommonEmoij.BEREND);
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leaves", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild, ErrorMessage = "This command is only possible via a public channel.")]
        public async Task LeaveCmd()
        {
            var result = await _service.LeaveAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
        }

        [Command("plays", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild, ErrorMessage = "This command is only possible via a public channel.")]
        public async Task PlayCmd([Remainder] string song)
        {
            var result = await _service.PlaySound(Context.Guild, Context.Channel, (Context.User as IVoiceState).VoiceChannel, song);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
        }

        [Command("stops", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild, ErrorMessage = "This command is only possible via a public channel.")]
        public async Task StopCmd()
        {
            var result = await _service.StopPlaying(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
        }

        [Command("say", RunMode = RunMode.Async)]
        [RequireContext(ContextType.Guild, ErrorMessage = "This command is only possible via a public channel.")]
        public async Task SayText([Remainder] string text)
        {
            var result = await _service.SayText(Context.Guild, (Context.User as IVoiceState).VoiceChannel,  Context.Channel, text);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
        }

        [Command("listaudio", RunMode = RunMode.Async)]
        public async Task ListAudioFiles()
        {
            var result = await _service.ListAudio(Context);
            await Context.Message.AddReactionAsync(result == AudioServiceStatus.Succes ? CommonEmoij.OK : CommonEmoij.NOK);
        }

    }
}
