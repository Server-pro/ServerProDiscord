using Discord;
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

        protected static List<CommandBase> _commands = new List<CommandBase>();

        private Code _code = Code.Success;
        protected string _name = "unnamed";
        protected string _description = "Description not set. ";
        private List<Argument> _arguments = new List<Argument>();

        protected static EmbedBuilder AllHelpEB = new EmbedBuilder();
        protected static List<EmbedBuilder> CommandHelpEB = new List<EmbedBuilder>();
        

        #region Init/Callback
        public static void Init()
        {
            AddCommand(new Help());
            AddCommand(new Ping());
            AddCommand(new SendRaw());
            AddCommand(new Suggestion());
            AllHelpEB.Title = "Commands";
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

            var temp = new EmbedBuilder();
            temp.Title = ("Command: " + command._name);
            temp.Description = command._description;

            foreach (var a in command._arguments)
            {
                temp.AddField($"Argument: {a.name[0]} {(a.required ? "(Required)" : "(Optional)")} ", a.description);
            }

            CommandHelpEB.Add(temp);

            AllHelpEB.AddField(command._name, command._description);
        }

        public static async Task CallCommands(SocketMessage sm, string prefix)
        {
            if (!sm.Content.StartsWith(prefix)) return;
            string msg = sm.Content.Substring(prefix.Length);

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
            _code = Code.Success;
            for(int i = 0; i < _arguments.Count; i++)
            {
                for(int h = 0; h < _arguments[i].name.Length; h++)
                {
                    //example: if the argument name is `test` then it will find a match in `!ping "test:value"` and invoke the delegate with "value"
                    Regex r = new Regex("-" + _arguments[i].name[h] + " .* ?", RegexOptions.Multiline);
                    Match match = r.Match(msg);

                    _arguments[i].SetSupplied(false);

                    if (match.Success)
                    {
                        _arguments[i].SetSupplied(true);
                        //strip off quotes, colon, and argument name
                        string value = match.Value.Substring(_arguments[i].name[h].Length + 1, match.Length - 1 - _arguments[i].name[h].Length);
                        _arguments[i].invoke.Invoke(value);
                    }
                }
            }

            CheckArguments();
            await Respond(sm, msg);
        }

        protected void AddArgument(string[] n, InvokeSig i, string d, bool r = false)
        {
            foreach (var a in _arguments)
            {
                if (a.name == n)
                {
                    Console.WriteLine($"Error: Duplicate register of argument '{n}' in command {GetType().Name.ToLower()}. Skipping second instance.");
                    return;
                }
            }
            _arguments.Add(new Argument(n, i, d, r));
        }

        protected void CheckArguments()
        {
            foreach(var a in _arguments)
            {
                if (a.required && !a.supplied) _code = Code.MissingArgument;
            }
        }

        protected bool HasArgument(string name)
        {
            foreach(var a in _arguments)
            {
                foreach(var n in a.name)
                {
                    if (n == name)
                        return a.supplied;
                }
            }
            return false;
        }
        #endregion 
        
        protected abstract Task Run(SocketMessage sm, string msg);

        public class Argument
        {
            public string[] name;
            public InvokeSig invoke;
            public string description;
            public bool required;
            public bool supplied;
            public Argument(string[] n, InvokeSig i, string d, bool r)
            {
                name = n;
                invoke = i;
                description = d;
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

