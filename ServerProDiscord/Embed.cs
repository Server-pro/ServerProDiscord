using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    static class Embed
    {
        public async static Task CheckEmbed(SocketMessage msg)
        {
            if (!msg.Content.StartsWith($"{Program.Config["Prefix"]}embed")) return;

            await CheckValid(msg);
            
        }

        private async static Task CheckValid(SocketMessage msg)
        {
            int index = msg.Content.IndexOf("`");

            if (index == -1)
            {
                Console.WriteLine("First index too close to end of block.");
                await SendInvalidFormat(msg);
                return;
            }

            var embed = msg.Content.Substring(index);

            index = embed.IndexOf("`");

            if (index > embed.Length - 2)
            {
                Console.WriteLine("First index too close to end of block.");
                await SendInvalidFormat(msg);
                return;
            }

            if (!embed[index + 1].Equals('`') || !embed[index + 2].Equals('`'))
            {
                Console.WriteLine("No opening backticks.");
                await SendInvalidFormat(msg);
                return;
            }

            embed = embed.Substring(3);

            int lastIndex = embed.LastIndexOf("`");

            if (lastIndex < 2)
            {
                Console.WriteLine("Last index too close to start of block");
                await SendInvalidFormat(msg);
                return;
            }

            if (!embed[lastIndex - 1].Equals('`') || !embed[lastIndex - 2].Equals('`'))
            {
                Console.WriteLine("No closing backticks");
                await SendInvalidFormat(msg);
                return;
            }

            embed = embed.Substring(0, lastIndex - 2);

            await Program.SendCustom(msg.Channel.Id, embed);
        }

        private async static Task SendInvalidFormat(SocketMessage msg)
        {
            await msg.Channel.SendMessageAsync(_invalidFormatMsg);
        }

        private const string _invalidFormatMsg = "Invalid code block.";
    }
}
