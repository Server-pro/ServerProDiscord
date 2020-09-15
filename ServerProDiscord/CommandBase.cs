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

        private Code _code = Code.Success;
        protected string _name;
        private List<Argument> _arguments = new List<Argument>();

        #region Init/Callback
        public static void Init()
        {
            AddCommand(new Ping());
            AddCommand(new SendRaw());
            AddCommand(new Suggestion());
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

        public async Task Respond(SocketMessage sm, string msg)
        {
            switch (_code)
            {
                case Code.Success:
                    {
                        await Run(sm, msg);
                        break;
                    }
                case Code.MissingArgument:
                    {
                        List<Argument> missing = _arguments.Where(c => c.required && !c.supplied).ToList();
                        StringBuilder sb = new StringBuilder("Missing the following args:\n");
                        foreach(var c in missing)
                        {
                            sb.Append($"\t- {c.name}");
                        }
                        await sm.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
            }
        }
        #endregion

        #region Arguments
        private async Task RunArguments(SocketMessage sm, string msg)
        {
            for(int i = 0; i < _arguments.Count; i++)
            {
                //example: if the argument name is `test` then it will find a match in `!ping "test:value"` and invoke the delegate with "value"
                Regex r = new Regex("\"" + _arguments[i].name + ":.*?\"", RegexOptions.Multiline);
                Match match = r.Match(msg);

                _arguments[i].SetSupplied(false);

                if (match.Success)
                {
                    _arguments[i].SetSupplied(true);
                    //strip off quotes, colon, and argument name
                    string value = match.Value.Substring(_arguments[i].name.Length + 2, match.Length - 3 - _arguments[i].name.Length);
                    _arguments[i].invoke.Invoke(value);
                }
            }

            await CheckArguments();
            await Respond(sm, msg);
        }

        protected void AddArgument(string n, InvokeSig i, bool r = false)
        {
            foreach (var a in _arguments)
            {
                if (a.name == n)
                {
                    Console.WriteLine($"Error: Duplicate register of argument '{n}' in command {GetType().Name.ToLower()}. Skipping second instance.");
                    return;
                }
            }
            _arguments.Add(new Argument(n, i, r));
        }

        protected async Task CheckArguments()
        {
            foreach(var a in _arguments)
            {
                if (a.required && !a.supplied) _code = Code.MissingArgument;
            }
        }
        #endregion 
        
        protected abstract Task Run(SocketMessage sm, string msg);

        public class Argument
        {
            public string name;
            public InvokeSig invoke;
            public bool required;
            public bool supplied;
            public Argument(string n, InvokeSig i, bool r = false)
            {
                name = n;
                invoke = i;
                required = r;
                supplied = false;
            }
            public void SetSupplied(bool val)
            {
                supplied = val;
            }
        }

        enum Code
        {
            Success = 0,
            MissingArgument = 1
        }
    }

}

