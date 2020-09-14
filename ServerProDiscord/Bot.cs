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
    class Bot
    {
        static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();

        private static DiscordSocketClient _client;

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += BlackList.DeleteBlackList;
            _client.MessageReceived += Embed.CheckEmbed;

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);

            //prepare in discord format then send to rcon
            string json = "{\"content\":\"" + msg.Message + "\"}";
            SendRaw.Send(RConChannel, json);

            return Task.CompletedTask;
        }

        #region Config Getters
        private static IConfiguration Config
        {
            get
            {
                if (_config != null) return _config;

                var _builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory + "../../../../")
                    .AddJsonFile(path: "config.json");
                return _config = _builder.Build();
            }
        }
        private static IConfiguration _config = null;
        public static string Prefix
        {
            get => DevEnv ? Config["DevPrefix"] : Config["Prefix"];
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
        #endregion 
    }
}
