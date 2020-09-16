using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class Support : CommandBase
    {
        public Support()
        {
            _name = GetType().Name.ToLower();
            _description = "Displays info on how to get support.";
            _example = "-c 755449336065818764";

            AddArgument(new string[]{"channel", "c"}, (value) =>
            {
                _channel = Convert.ToUInt64(value);
            }, "The channel in which the bot will respond");
        }

        private ulong _channel = 0;

        protected override async Task Run(SocketMessage sm, string msg) 
            => await Bot.Instance.Send(_channel == 0 ? sm.Channel.Id : _channel, "For support, please contact us via https://server.pro/contact or email support@server.pro. We do not offer support over Discord.");
        protected override bool HasPermission(string id) => true;
    }
}
