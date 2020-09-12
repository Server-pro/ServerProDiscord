using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    static class BlackList
    {
        public static readonly string BlackListPath = "../../../../blacklist.txt";
        public static List<string> _blackList;
        public static async Task DeleteBlackList(SocketMessage msg)
        {
            foreach (string s in _blackList)
            {
                if (msg.Content.ToLower().Contains(s))
                {
                    await msg.DeleteAsync();
                    await WarnBlackList(msg);
                    return;
                }
            }
        }

        public static async Task WarnBlackList(SocketMessage msg)
        {
            await msg.Channel.SendMessageAsync($"{msg.Author.Mention} has been warned!");
        }


    }

}
