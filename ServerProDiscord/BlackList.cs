using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    static class BlackList
    {
        public static async Task DeleteBlackList(SocketMessage msg)
        {
            if (List.Contains(msg.Content))
            {
                await msg.DeleteAsync();
                await WarnBlackList(msg);
            }
        }

        public static async Task WarnBlackList(SocketMessage msg)
        {
            //Warn logic here
            await msg.Channel.SendMessageAsync($"{msg.Author.Mention} has been warned!");
        }

        public static List<string> List
        {
            get => _list ??= System.IO.File.ReadLines("../../../../blacklist.txt").ToList();
        }
        private static List<string> _list;
    }

}
