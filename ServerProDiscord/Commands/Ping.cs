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
            AddArgument("channel", (value) =>
            {
                _channel = Convert.ToUInt64(value);
            });
        }

        private ulong _channel = 0;

        protected override async Task Run(SocketMessage sm, string msg) => await Bot.Instance.Send(_channel == 0 ? sm.Channel.Id : _channel, msg);
    }
}
