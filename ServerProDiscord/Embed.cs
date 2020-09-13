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
            if (msg.Content.StartsWith($"{Program.Prefix}embed"))
            {
                Console.WriteLine("Embed command found");
                await CheckValid(msg);
            }
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

            await SendEmbed(msg, embed);
        }

        private async static Task SendInvalidFormat(SocketMessage msg)
        {
            await msg.Channel.SendMessageAsync(_invalidFormatMsg);
        }

        private async static Task SendEmbed(SocketMessage msg, string embed)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(embed, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bot " + Program.Token);
            Console.WriteLine($"Sending:\n {embed}");
            var response = client.PostAsync($"https://discord.com/api/channels/{msg.Channel.Id}/messages", content);
            Console.WriteLine(response.Result.StatusCode.ToString());
            await Task.Delay(0);
        }

        private const string _invalidFormatMsg = "Invalid code block.";
    }
}
