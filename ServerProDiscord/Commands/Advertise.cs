using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord.Commands
{
    class Advertise : CommandBase
    {
        public Advertise()
        {
            _name = GetType().Name.ToLower();
            _description = "Creates an ad for your server in the server ads channel.";
            _example = "-id server_id -d \"Join our server!\"";
            AddArgument(new string[] { "id" }, (value) =>
             {
                 _id = value;
             }, "The id of your server. This can be found in your url when in your control panel.", true);
            AddArgument(new string[] { "description", "d", "desc" }, (value) =>
            {
                _desc = value;
            }, "Custom message to add to your advertisement", false);
        }

        string _id;
        string _desc = null;

        protected override async Task Run(SocketMessage sm, string msg)
        {
            Server server = Server.GetServerFromID(_id);
            Discord.EmbedBuilder eb = new Discord.EmbedBuilder()
                .WithTitle(server.Name)
                .AddField("Type", server.Type, true)
                .AddField("Location", server.Location, true);

            if (_desc != null)
                eb.Description = _desc;

            await Bot.Instance.Send(Config.Profile.AdChannel, embed: eb.Build());
        }
        protected override bool HasPermission(SocketUser user) => false;
    }
}
