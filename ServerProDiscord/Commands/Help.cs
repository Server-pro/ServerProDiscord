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
        public Help()
        {
            _name = GetType().Name.ToLower();
            _description = "List all commands. Use 'help -c command' for info on a specific command.";
            _example = "-c help";

            AddArgument(new string[]{"command", "c"}, (value) =>
            {
                command = value;
            }, "Get information on a specific command instead.");
        }

        private string command;

        protected override async Task Run(SocketMessage sm, string content)
        {
            if (HasArgument("command"))
                await sm.Channel.SendMessageAsync(embed: CommandHelpEB.Where(eb => eb.Title.Contains(command)).First()?.Build());
            else
                await sm.Channel.SendMessageAsync(embed: AllHelpEB.Build());
        }
    }
}
