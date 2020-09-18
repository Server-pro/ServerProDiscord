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
    public class Bot
    {
        public static Bot Instance;
        private static void Main() {
            Instance = new Bot();
            Instance.MainAsync().GetAwaiter().GetResult();
        }

        public DiscordSocketClient _client;
        public MessageHandler _messageHandler;

        private BlackList _blackList;

        public async Task<Discord.Rest.RestUserMessage> Send(ulong channel, string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await ((SocketTextChannel)_client.GetChannel(channel)).SendMessageAsync(message, isTTS, embed, options);
        }

        private async Task BotCallCommands(SocketMessage sm) => await CommandBase.CallCommands(sm, Config.Profile.Prefix);

        private async Task MainAsync()
        {
            Config.Init();
            CommandBase.Init(Config.Profile.Prefix);

            _client = new DiscordSocketClient();
            _blackList = new BlackList("../../../../blacklist.txt");
            _messageHandler = new MessageHandler(Config.Token);

            _client.Log += Log;
            _client.MessageReceived += _blackList.Check;
            _client.MessageReceived += BotCallCommands;

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);

            //prepare in discord format then send to rcon
            //string json = "{\"content\":\"" + msg.Message + "\"}";
            //_messageHandler.Send(RConChannel, json);

            return Task.CompletedTask;
        }
    }
}
