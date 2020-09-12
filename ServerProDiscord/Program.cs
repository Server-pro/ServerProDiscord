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

            string token = System.IO.File.ReadAllText(TokenPath);
            BlackList._blackList = System.IO.File.ReadLines(BlackList.BlackListPath).ToList();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }



        static readonly string TokenPath = "../../../../.token";
    }
}
