using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class Help : CommandBase
    {
        public Help(EmbedBuilder all, List<EmbedBuilder> single)
        {
            _all = all;
            _single = single;

            _name = GetType().Name.ToLower();
            _description = "List all commands. Use 'help -c command' for info on a specific command.";
            _example = "-c help";

            AddArgument(new string[]{"command", "c"}, (value) =>
            {
                _command = value;
            }, "Get information on a specific command instead.");
        }

        private string _command;
        private EmbedBuilder _all;
        private List<EmbedBuilder> _single;

        protected override async Task Run(SocketMessage sm, string content)
        {
            if (HasArgument("command"))
            {
                var EBList = _single.Where(eb => eb.Title.Contains(_command)).ToList();
                if (EBList.Count == 0) await sm.Channel.SendMessageAsync($"Command '{_command}' not found.");
                await sm.Channel.SendMessageAsync(embed: EBList.First().Build());
            }
            else
                await sm.Channel.SendMessageAsync(embed: _all.Build());
        }
        protected override bool HasPermission(ulong id) => true;
    }
}
