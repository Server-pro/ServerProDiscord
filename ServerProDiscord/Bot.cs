﻿using System;
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
        private static void Main() => new Bot().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private BlackList _blackList;
        private MessageHandler _messageHandler;

        #region Config Getters
        private IConfiguration Config
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
        private IConfiguration _config = null;
        public string Prefix
        {
            get => DevEnv ? Config["DevPrefix"] : Config["Prefix"];
        }
        public bool DevEnv
        {
            get => Convert.ToBoolean(Config["DevEnv"]);
        }

        public ulong RConChannel
        {
            get => DevEnv ? ulong.Parse(Config["DevRConChannel"]) : ulong.Parse(Config["RConChannel"]);
        }

        public string Token
        {
            get => DevEnv ? Config["DevToken"] : Config["Token"];
        }
        #endregion 

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _blackList = new BlackList("../../../../blacklist.txt");
            _messageHandler = new MessageHandler(Token);

            _client.Log += Log;
            _client.MessageReceived += _blackList.Check;
            _client.MessageReceived += CallCommands;

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Checks for text commands and calls the respective method.
        /// </summary>
        /// <param name="msg">Message sent by discord.</param>
        /// <returns>Nothing.</returns>
        private async Task CallCommands(SocketMessage msg)
        {
            if (!msg.Content.StartsWith(Prefix)) return;
            string content = msg.Content.Substring(Prefix.Length);

            if (content.StartsWith("sendcustom"))  await _messageHandler.CheckBlock(msg);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);

            //prepare in discord format then send to rcon
            string json = "{\"content\":\"" + msg.Message + "\"}";
            _messageHandler.Send(RConChannel, json);

            return Task.CompletedTask;
        }
    }
}