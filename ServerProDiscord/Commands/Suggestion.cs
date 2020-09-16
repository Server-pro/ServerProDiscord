﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class Suggestion : CommandBase
    {
        public Suggestion()
        {
            _name = GetType().Name.ToLower();
            _description = "Formally send a suggestion to the suggestions channel. Adds reactions to vote on the suggestion.";
            _example = "-t AwesomeSuggestion -b AwesomeDescription";
            
            AddArgument(new string[] { "title", "t" }, (val) =>
            {
                title = val;
            }, "The title. Very brief explanation of your suggestion.", true);
            AddArgument(new string[] { "body", "b"}, (val) =>
            {
                body = val;
            }, "The body portion of your suggestion. Describe your suggestion in detail here.", true);
        }

        string title = null;
        string body = null;
        private readonly Emoji yes = new Emoji("😊");
        private readonly Emoji no = new Emoji("🤮");

        #region Config
        private IConfiguration Config
        {
            get
            {
                if (_config != null) return _config;

                var _builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory + "../../../../")
                    .AddJsonFile(path: "suggestionconfig.json");
                return _config = _builder.Build();
            }
        }
        private IConfiguration _config;
        private ulong SuggestionChannel
        {
            get
            {
                if (_suggestionChannel == 0)
                {
                    return _suggestionChannel = Convert.ToUInt64(Bot.Instance.DevEnv ? Config["DevSuggestionChannel"] : Config["SuggestionChannel"]);
                }
                else return _suggestionChannel;
            }
        }
        private ulong _suggestionChannel = 0;
        #endregion

        protected override async Task Run(SocketMessage sm, string msg)
        {
            EmbedBuilder eb = new Discord.EmbedBuilder
            {
                Title = title,
                Description = body
            };
            var message = await Bot.Instance.Send(SuggestionChannel, embed: eb.Build());
            await message.AddReactionAsync(yes);
            await message.AddReactionAsync(no);
        }
    }
}