using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class SendCustom : CommandBase
    {
        public SendCustom()
        {
            _call = "sendcustom";
            AddArgument("channel", (val) =>
            {
                channel = Convert.ToUInt64(val);
            });
        }

        private ulong channel = 0;

        protected override async Task Run(SocketMessage sm, string msg)
        {
            Regex codeBlock = new Regex("^`{3}.*`{3}$", RegexOptions.Multiline);
            var match = codeBlock.Match(sm.Content);

            if (!match.Success)
            {
                await sm.Channel.SendMessageAsync("Invalid code block.");
                return;
            }

            string content = match.Value.Substring(3, match.Length - 6);
            Bot.Instance._messageHandler.Send(channel == 0 ? sm.Channel.Id : channel, content);
        }
    }
}
