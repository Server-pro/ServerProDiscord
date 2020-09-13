using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ServerProDiscord
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.MessageReceived += BlackList.DeleteBlackList;
            _client.MessageReceived += Embed.CheckEmbed;

            Token = System.IO.File.ReadAllText(_tokenPath);
            BlackList._blackList = System.IO.File.ReadLines(BlackList.BlackListPath).ToList();

            Prefix = System.IO.File.ReadAllText(_prefixPath);

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        public static string Prefix = "!";
        private const string _prefixPath = "../../../../prefix.txt";
        public static string Token = "";
        private const string _tokenPath = "../../../../token.txt";
    }
}
