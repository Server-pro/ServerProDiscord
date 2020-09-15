using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class Ping : CommandBase
    {
        public Ping()
        {
            _name = GetType().Name.ToLower();
            _description = "Testing command often used to verify if the bot is online.";

            AddArgument(new string[]{"channel", "c"}, (value) =>
            {
                _channel = Convert.ToUInt64(value);
            }, "The channel in which the bot will respond");
        }

        private ulong _channel = 0;

        protected override async Task Run(SocketMessage sm, string msg) => await Bot.Instance.Send(_channel == 0 ? sm.Channel.Id : _channel, msg);
    }
}
