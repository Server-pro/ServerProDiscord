using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ServerProDiscord
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public static IConfiguration Config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += BlackList.DeleteBlackList;
            _client.MessageReceived += Embed.CheckEmbed;

            var _builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory + "../../../../").AddJsonFile(path: _configPath);
            Config = _builder.Build();

            BlackList._blackList = System.IO.File.ReadLines(BlackList.BlackListPath).ToList();

            await _client.LoginAsync(TokenType.Bot, Config["Token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            string json = "{\"content\":\"" + msg.Message + "\"}";
            SendCustom(BotRConID, json);
            return Task.CompletedTask;
        }

        public static void SendCustom(ulong channel, string json)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bot " + Program.Config["Token"]);
            var response = client.PostAsync($"https://discord.com/api/channels/{channel}/messages", content);
            if(response.Result.StatusCode.ToString() != "OK") Console.WriteLine(response.Result.StatusCode);
        }

        private const string _configPath = "config.json";

        public const ulong BotRConID = 754748855119118377;
        public const ulong BotTestID = 754413667365421058;
    }
}
