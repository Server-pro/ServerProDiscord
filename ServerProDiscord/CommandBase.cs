using Discord.WebSocket;
using ServerProDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    abstract class CommandBase
    {
        public delegate void InvokeSig(string value);

        private static List<CommandBase> _registered = new List<CommandBase>();

        protected string _call;

        private List<Argument> _arguments = new List<Argument>();

        public static void Init()
        {
            _registered.Add(new Ping());
            _registered.Add(new SendCustom());
        }

        public static async Task CallCommands(SocketMessage sm, string prefix)
        {
            string msg = sm.Content.Substring(prefix.Length).ToLower();

            foreach(var c in _registered)
            {
                if(msg.StartsWith(c._call)) await c.RunArguments(sm, msg.Substring(c._call.Length));
            }
            await Task.CompletedTask;
        }

        protected async Task RunArguments(SocketMessage sm, string msg)
        {
            foreach(var a in _arguments)
            {
                //example: if the argument name is `test` then it will find a match in `!ping "test:value"` and invoke the delegate with "value"
                Regex r = new Regex("\"" + a.name + ":.*\"", RegexOptions.Multiline);
                Match match = r.Match(msg);

                if (match.Success)
                {
                    //strip off quotes, colon, and argument name
                    string value = match.Value.Substring(a.name.Length + 2, match.Length - 3 - a.name.Length);
                    a.invoke.Invoke(value);
                }
            }

            await Run(sm, msg);
        }

        protected abstract Task Run(SocketMessage sm, string msg);

        protected void AddArgument(string n, InvokeSig i) => _arguments.Add(new Argument(n, i));

        public struct Argument
        {
            public string name;
            public InvokeSig invoke;
            public Argument(string n, InvokeSig i)
            {
                name = n;
                invoke = i;
            }
        }
    }

}

