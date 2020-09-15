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
            _call = "ping";
            AddArgument("channel", (value) =>
            {
                _channel = Convert.ToUInt64(value);
            });
        }

        private ulong _channel = 0;

        protected override async Task Run(SocketMessage sm, string msg) => await ((SocketTextChannel)Bot.Instance._client.GetChannel(_channel == 0 ? sm.Channel.Id : _channel)).SendMessageAsync("pong");
    }
}
