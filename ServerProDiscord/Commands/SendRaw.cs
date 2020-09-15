using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class SendRaw : CommandBase
    {
        public SendRaw()
        {
            _name = GetType().Name.ToLower();
            AddArgument("channel", (val) =>
            {
                channel = Convert.ToUInt64(val);
            });
        }

        private ulong channel = 0;

        protected override async Task Run(SocketMessage sm, string msg)
        {
            bool isCodeBlock = Bot.Instance._messageHandler.StripCodeBlock(sm.Content, out string content);
            if (isCodeBlock) Bot.Instance._messageHandler.SendRaw(channel == 0 ? sm.Channel.Id : channel, content);
            else await Bot.Instance.Send(sm.Channel.Id, "Code block missing.");
        }
    }
}
