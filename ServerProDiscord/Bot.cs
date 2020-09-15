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
        public static Bot Instance;
        private static void Main() {
            Instance = new Bot();
            Instance.MainAsync().GetAwaiter().GetResult();
        }

        public DiscordSocketClient _client;
        private BlackList _blackList;
        public MessageHandler _messageHandler;

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

        private async Task BotCallCommands(SocketMessage sm) => await CommandBase.CallCommands(sm, Prefix);

        private async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _blackList = new BlackList("../../../../blacklist.txt");
            _messageHandler = new MessageHandler(Token);
            CommandBase.Init();

            _client.Log += Log;
            _client.MessageReceived += _blackList.Check;
            _client.MessageReceived += BotCallCommands;

            await _client.LoginAsync(TokenType.Bot, Token);
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
