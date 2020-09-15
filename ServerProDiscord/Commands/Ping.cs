using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerProDiscord.Commands
{
    class Ping : CommandBase
    {
        public Ping()
        {
            _call = "ping";
            AddArgument("channel", (value) =>
            {
                _channel = Convert.ToUInt64(value);
            });
        }

        private ulong _channel = 0;

        protected override void Run(SocketMessage sm, string msg) => ((SocketTextChannel)Bot.Instance._client.GetChannel(_channel == 0 ? sm.Channel.Id : _channel)).SendMessageAsync("pong");
    }
}
