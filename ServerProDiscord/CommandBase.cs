using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ServerProDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    /// <summary>
    /// Base class for any command. Must register new commands in Init(). Provides regex searching for arguments.
    /// </summary>
    abstract class CommandBase
    {
        public delegate void InvokeSig(string value);

        private static readonly List<CommandBase> _commands = new List<CommandBase>();
        private static readonly EmbedBuilder _globalHelpEmbed = new EmbedBuilder();
        private static readonly List<EmbedBuilder> _instanceHelpEmbed = new List<EmbedBuilder>();

        protected string _name = "unnamed";
        protected string _description = "No description set.";
        protected string _example = "No example set.";

        private Code _code = Code.Success;
        private readonly List<Argument> _arguments = new List<Argument>();
        

        #region Init/Callback
        public static void Init()
        {
            _globalHelpEmbed.Title = "Commands";
            AddCommand(new Help(_globalHelpEmbed, _instanceHelpEmbed));
            AddCommand(new Ping());
            AddCommand(new SendRaw());
            AddCommand(new Suggestion());

            //Admin commands
        }

        /// <summary>
        /// Registers the given command and initializes its embed for the help command. Includes duplicate checking.
        /// </summary>
        /// <param name="command">The command to register.</param>
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
            temp.Title = ("Command: " + Bot.Instance.Prefix + command._name);
            temp.Description = command._description;

            foreach (var a in command._arguments)
            {
                string aliases = "";
                foreach (var n in a.name) aliases += $"{n}, ";
                temp.AddField($"Argument: {aliases.Substring(0, aliases.Length -2)} {(a.required ? "(Required)" : "(Optional)")} ", a.description);
            }
            temp.AddField("Example:", $"{Bot.Instance.Prefix}{command._name} {command._example}");

            _instanceHelpEmbed.Add(temp);

            _globalHelpEmbed.AddField(Bot.Instance.Prefix + command._name, command._description);
        }

        /// <summary>
        /// Checks every message against all commands and executes if necessary.
        /// </summary>
        /// <param name="sm">Unaltered socket message reveived.</param>
        /// <param name="prefix">Supply bot's prefix here.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks for any errors in the command. If nothing is found, runs the body of the command.
        /// </summary>
        /// <param name="sm">Unaltered socket message reveived.</param>
        /// <param name="msg">The message with the prefix and command name stripped off.</param>
        /// <returns></returns>
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
                            sb.Append($"\t- {c.name[0]}\n");
                        }
                        await sm.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
            }
        }
        #endregion

        #region Arguments
        /// <summary>
        /// Checks for arguments in the message and executes the respective delegates if found.
        /// </summary>
        /// <param name="sm">Unaltered socket message.</param>
        /// <param name="msg">Content with prefix and command name stripped off.</param>
        /// <returns></returns>
        private async Task RunArguments(SocketMessage sm, string msg)
        {
            _code = Code.Success;
            for(int i = 0; i < _arguments.Count; i++)
            {
                _arguments[i].SetSupplied(false);
                for (int h = 0; h < _arguments[i].name.Length; h++)
                {
                    //example: if the argument name is `test` then it will find a match in `!ping "test:value"` and invoke the delegate with "value"
                    Regex Quotes = new Regex($"-{_arguments[i].name[h]} \"[^\"]*", RegexOptions.Multiline);
                    Regex NoQuotes = new Regex($"-{_arguments[i].name[h]} [^ ]*", RegexOptions.Multiline);
                    Match match = Quotes.Match(msg);

                    bool IsQuotes = true;
                    if (!match.Success)
                    {
                        match = NoQuotes.Match(msg);
                        IsQuotes = false;
                    }

                    if (match.Success)
                    {
                        _arguments[i].SetSupplied(true);

                        //strip off argument name, dash, space, quotes
                        int startIndex = _arguments[i].name[h].Length + 2 + (IsQuotes ? 1 : 0);
                        int length = match.Length - _arguments[i].name[h].Length - 2 - (IsQuotes ? 1 : 0);

                        string value = match.Value.Substring(startIndex, length);
                        _arguments[i].invoke.Invoke(value);
                    }
                }
            }

            CheckArguments();
            await Respond(sm, msg);
        }

        /// <summary>
        /// The interface for a derived command to add an argument.
        /// </summary>
        /// <param name="n">The name.</param>
        /// <param name="i">The delegate (invoke).</param>
        /// <param name="d">The description (For help command).</param>
        /// <param name="r">Required. If true the command will fail if it is not supplied.</param>
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

        /// <summary>
        /// Checks for missing required arguments. If it finds any it changes the error code.
        /// </summary>
        protected void CheckArguments()
        {
            foreach(var a in _arguments)
            {
                if (a.required && !a.supplied) _code = Code.MissingArgument;
            }
        }

        /// <summary>
        /// Checks if an argument was supplied.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <returns></returns>
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

        #region Permissions
        protected IConfiguration Permissions
        {
            get
            {
                if (_permissions != null) return _permissions;

                var _builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory + "../../../../")
                    .AddJsonFile(path: "permissions.json");
                return _permissions = _builder.Build();
            }
        }
        private IConfiguration _permissions;
        protected bool IsAdmin(ulong UserID)
        {
            for(int i = 0; i < Permissions["admin"].Length; i++)
            {
                if (UserID == Convert.ToUInt64(Permissions["admin"][i]))
                    return true;
            }
            return false;
        }
        #endregion

        protected abstract Task Run(SocketMessage sm, string msg);
        protected bool HasPermission() => true;

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

