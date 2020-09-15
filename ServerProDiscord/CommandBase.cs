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

        private static List<CommandBase> _commands = new List<CommandBase>();

        protected string _name;
        private List<Argument> _arguments = new List<Argument>();

        #region Init/Callback
        public static void Init()
        {
            AddCommand(new Ping());
            AddCommand(new SendRaw());
        }

        private static void AddCommand(CommandBase command)
        {
            foreach(var c in _commands)
            {
                if(c._name == command._name)
                {
                    Console.WriteLine($"Error: Duplicate register of command '{c._name}'. Skipping second instance.");
                    return;
                }
            }
            _commands.Add(command);
        }

        public static async Task CallCommands(SocketMessage sm, string prefix)
        {
            string msg = sm.Content.Substring(prefix.Length).ToLower();

            foreach(var c in _commands)
            {
                if(msg.StartsWith(c._name)) await c.RunArguments(sm, msg.Substring(c._name.Length));
            }
            await Task.CompletedTask;
        }
        #endregion

        #region Arguments
        private async Task RunArguments(SocketMessage sm, string msg)
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

        protected void AddArgument(string n, InvokeSig i)
        {
            foreach (var a in _arguments)
            {
                if (a.name == n)
                {
                    Console.WriteLine($"Error: Duplicate register of argument '{n}' in command {GetType().Name.ToLower()}. Skipping second instance.");
                    return;
                }
            }
            _arguments.Add(new Argument(n, i));
        }
        #endregion 
        
        protected abstract Task Run(SocketMessage sm, string msg);

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

