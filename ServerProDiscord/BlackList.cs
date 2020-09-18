using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    class BlackList
    {
        private List<string> _list;

        public BlackList(string path) => _list = System.IO.File.ReadLines(path).ToList();

        public async Task Check(SocketMessage msg)
        {
            if (_list.Contains(msg.Content))
            {
                await msg.DeleteAsync();
                await Warn(msg);
            }
        }

        private async Task Warn(SocketMessage msg)
        {
            //Warn logic here
            await msg.Channel.SendMessageAsync($"{msg.Author.Mention} has been warned!");
        }
    }

}
