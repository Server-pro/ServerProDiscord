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

        private static DiscordSocketClient _client;

        private static IConfiguration Config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += BlackList.DeleteBlackList;
            _client.MessageReceived += Embed.CheckEmbed;

            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory + "../../../../")
                .AddJsonFile(path: "config.json");
            Config = _builder.Build();

            BlackList._blackList = System.IO.File.ReadLines(BlackList.BlackListPath).ToList();

            SendRaw.Init();

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            string json = "{\"content\":\"" + msg.Message + "\"}";
            SendRaw.Send(RConChannel, json);
            return Task.CompletedTask;
        }

        public static bool DevEnv
        {
            get => Convert.ToBoolean(Config["DevEnv"]);
        }

        public static ulong RConChannel
        {
            get => DevEnv ? ulong.Parse(Config["DevRConChannel"]) : ulong.Parse(Config["RConChannel"]);
        }

        public static string Token 
        { 
            get => DevEnv ? Config["DevToken"] : Config["Token"];
        }
    }
}
