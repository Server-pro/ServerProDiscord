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
            _description = "Sends a manual POST request to the discord api from the bot. The content of this request must be properly json formatted in a code block.";
            _example = "-c 754493019020984431 ```{\"content\":\"testing\"}```";

            AddArgument(new string[] { "channel" }, (val) =>
            {
                channel = Convert.ToUInt64(val);
            }, "The channel in which the bot will respond");
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
