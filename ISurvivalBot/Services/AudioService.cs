using Discord;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Overby.Extensions.AsyncBinaryReaderWriter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{

    public enum AudioServiceStatus
    {
        Succes,
        Failure
    }


    public class AudioServiceState
    {
        public ulong VoiceChatId { get; set; }
        public IAudioClient AudioClient { get; set; }

        public ConcurrentQueue<string> PlayList { get; set; } = new ConcurrentQueue<string>();

        public bool IsPlaying { get; set; } = false;

        public CancellationTokenSource? CancellationTokenSource { get; set; } = null;

        public IVoiceChannel VoiceChannel { get; set; }
    }
    public class AudioService
    {

        private readonly ConcurrentDictionary<ulong, AudioServiceState> AudioState = new ConcurrentDictionary<ulong, AudioServiceState>(); 

        private readonly ILogger<AudioService> _logger;

        public AudioService(ILogger<AudioService> logger) => _logger = logger;

        public async Task<AudioServiceStatus> JoinAudio(IGuild guild, IVoiceChannel voiceChannel)
        {
            AudioServiceState audioServiceState;
            if (AudioState.TryGetValue(guild.Id, out audioServiceState)) {
                _logger.LogError($"Guild {guild.Name} is already connected or queue exist");
                return AudioServiceStatus.Failure;
            }
            if (voiceChannel == null ||  voiceChannel.Guild.Id != guild.Id) {
                return AudioServiceStatus.Failure;
            }

            var audioClient = await voiceChannel.ConnectAsync();

            audioServiceState = new AudioServiceState()
            {
                AudioClient = audioClient,
                VoiceChannel = voiceChannel,
            };

            if (AudioState.TryAdd(guild.Id, audioServiceState))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                audioClient.Connected += async () =>
                {
                    _logger.LogInformation($"Connected to voice channel: {voiceChannel.Name}");
                };
                audioClient.Disconnected += async (Exception ex) =>
                {
                    _logger.LogError($"Disconnected from voice channel: {voiceChannel.Name} with exception: {ex.Message}");
                };
                _logger.LogInformation($"Connected to voice channel: {guild.Name}.");
            }

            return AudioServiceStatus.Succes;
        }


        public async Task<AudioServiceStatus> LeaveAudio(IGuild guild, IVoiceChannel voiceChannel)
        {
            if (voiceChannel == null || voiceChannel.Guild.Id != guild.Id)
            {
                return AudioServiceStatus.Failure;
            }
            AudioServiceState audioState = await getAudioState(guild);
            if (audioState == null)
            {
                _logger.LogInformation("Not connected");
                return AudioServiceStatus.Failure;
            }
            if (audioState.IsPlaying)
                await StopPlaying(guild, voiceChannel);
            _logger.LogInformation($"Disconnected from voice on {guild.Name}.");
            await audioState.AudioClient.StopAsync();
            audioState.IsPlaying = false;

            AudioState.Remove(guild.Id, out audioState);

            return AudioServiceStatus.Succes;
        }


        public async Task<AudioServiceStatus> PlaySound(IGuild guild, IMessageChannel channel, IVoiceChannel voiceChannel, string path)
        {
            AudioServiceState audioState = await getAudioState(guild);
            if (audioState == null || audioState.IsPlaying)
            {
                Console.WriteLine("Is already playing");
                return AudioServiceStatus.Failure;
            }
            if (voiceChannel == null || voiceChannel.Guild.Id != guild.Id)
            {
                return AudioServiceStatus.Failure;
            }

            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                _logger.LogInformation("Not connected");
                return AudioServiceStatus.Failure;
            }


            //IAudioClient client;
            //if (ConnectedChannels.TryGetValue(guild.Id, out client)) {

            audioState.IsPlaying = true;
            await audioState.AudioClient.SetSpeakingAsync(true); // send a speaking indicator

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };


            using (var ffmpeg = Process.Start(psi))
            using (var discord = audioState.AudioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    audioState.CancellationTokenSource = new CancellationTokenSource();
                    audioState.CancellationTokenSource.Token.Register(() => ffmpeg.Kill());
                    var output = ffmpeg.StandardOutput.BaseStream;
                    await output.CopyToAsync(discord, audioState.CancellationTokenSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogWarning("AudioService: Operation Cancelled");
                }
                finally
                {
                    await discord.FlushAsync();
                    audioState.IsPlaying = false;
                    await audioState.AudioClient.SetSpeakingAsync(false); // we're not speaking anymore
                    ffmpeg.Kill();
                }
            }
            //}
            return AudioServiceStatus.Succes;

        }

        public async Task<AudioServiceStatus> ListAudio(ICommandContext context)
        {
            var audioFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.mp3")
                .Where(file => new string[] { ".mp3", ".wav" }
                .Contains(Path.GetExtension(file)))
                .ToList();


            string message = $"Available files: " + String.Join(", ", audioFiles.ToArray());
            if (message.Length > 1950)
            {
                message = message.Substring(0, 1950) + "....";
            }
            await context.User.SendMessageAsync(message);
            return AudioServiceStatus.Succes;
        }

        public async Task<AudioServiceStatus> StopPlaying(IGuild guild, IVoiceChannel voiceChannel) {

            AudioServiceState audioState = await getAudioState(guild);
            if (audioState == null || audioState.IsPlaying)
            {
                Console.WriteLine("Is already playing");
                return AudioServiceStatus.Failure;
            }
            if (voiceChannel.Guild.Id != guild.Id)
            {
                return AudioServiceStatus.Failure;
            }

            if (audioState.CancellationTokenSource == null) {
                return AudioServiceStatus.Failure;
            }
            audioState.CancellationTokenSource.Cancel();
            _logger.LogWarning("Cancel token");
            return AudioServiceStatus.Succes;
        }


        private async Task PrintAvailableTTS(SpeechSynthesizer synth)
        {
            // Output information about all of the installed voices.   
            Console.WriteLine("Installed voices -");
            foreach (InstalledVoice voice in synth.GetInstalledVoices()) {
                VoiceInfo info = voice.VoiceInfo;
                string AudioFormats = "";
                foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats) {
                    AudioFormats += String.Format("{0}\n",
                    fmt.EncodingFormat.ToString());
                }

                Console.WriteLine(" Name:          " + info.Name);
                Console.WriteLine(" Culture:       " + info.Culture);
                Console.WriteLine(" Age:           " + info.Age);
                Console.WriteLine(" Gender:        " + info.Gender);
                Console.WriteLine(" Description:   " + info.Description);
                Console.WriteLine(" ID:            " + info.Id);
                Console.WriteLine(" Enabled:       " + voice.Enabled);
                if (info.SupportedAudioFormats.Count != 0)
                    Console.WriteLine(" Audio formats: " + AudioFormats);
                else
                    Console.WriteLine(" No supported audio formats found");

                string AdditionalInfo = "";
                foreach (string key in info.AdditionalInfo.Keys) {
                    AdditionalInfo += String.Format("  {0}: {1}\n", key, info.AdditionalInfo[key]);
                }

                Console.WriteLine(" Additional Info - " + AdditionalInfo);
            }
        }


        private async Task<AudioServiceState> getAudioState(IGuild guild)
        {
            AudioServiceState audioServiceState;
            if (AudioState.TryGetValue(guild.Id, out audioServiceState))
            {
                return audioServiceState;
            }
            return null;
        }



        public async Task<AudioServiceStatus> SayText(IGuild guild, IVoiceChannel voiceChannel, IMessageChannel channel, string text)
        {

            AudioServiceState audioState = await getAudioState(guild);
            if (audioState == null || audioState.IsPlaying) {
                Console.WriteLine("Is already playing");
                return AudioServiceStatus.Failure;
            }
            if (voiceChannel == null || voiceChannel.Guild.Id != guild.Id)
            {
                return AudioServiceStatus.Failure;
            }

            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer()) {
                await PrintAvailableTTS(synth);
                synth.SetOutputToWaveFile("D:\\output.wav");


                synth.Speak(text);

                return await PlaySound(guild, channel, voiceChannel, "D:\\output.wav");
            }

        }

      
    }
}
