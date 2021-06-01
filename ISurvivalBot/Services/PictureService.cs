using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ISurvivalBot.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public PictureService(HttpClient http, IConfiguration config)
        { 
             _http = http;
            _config = config;
        }


        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetPandaPictureAsync()
        {
            var resp = await _http.GetAsync("https://redpanda.pics/");
            return await resp.Content.ReadAsStreamAsync();
        }


        public async Task<Stream> GetDogPictureAsync()
        {

            var resp = await _http.GetAsync($"https://api.thedogapi.com/v1/images/search?api_key={_config["DogKey"]}");
            var responseBody = await resp.Content.ReadAsStringAsync();
            dynamic body = JsonConvert.DeserializeObject(responseBody);

            var resp1 = await _http.GetAsync($"{body[0].url}");
            return await resp1.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetFoxPictureAsync()
        {

            var resp = await _http.GetAsync($"https://randomfox.ca/floof/");
            var responseBody = await resp.Content.ReadAsStringAsync();
            dynamic body = JsonConvert.DeserializeObject(responseBody);

            var resp1 = await _http.GetAsync($"{body.image}");
            return await resp1.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetCatFact()
        {
            var resp = await _http.GetAsync($"https://meowfacts.herokuapp.com/");
            var responseBody = await resp.Content.ReadAsStringAsync();
            dynamic body = JsonConvert.DeserializeObject(responseBody);
            var body1 = body.data;
            return body1[0];
        }
    }
}
