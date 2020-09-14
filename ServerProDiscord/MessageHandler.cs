using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    public class MessageHandler
    {
        private HttpClient _client;

        public MessageHandler(string token)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bot " + token);
        }

        /// <summary>
        /// Sends a sends a custom message using json format. Prints response code if not "OK."
        /// </summary>
        /// <param name="channel">The channel to send the message to.</param>
        /// <param name="json">The formatted json sent in the request. Not altered in the method.</param>
        /// <returns>Returns the HttpResponseMessage from the api.</returns>
        public void Send(ulong channel, string json)
        {
            //build and send request
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var _response = _client.PostAsync($"https://discord.com/api/channels/{channel}/messages", content);

            //log error codes
            if (_response.Result.StatusCode.ToString() != "OK") Console.WriteLine(_response.Result.StatusCode);
        }

        public async Task CheckBlock(SocketMessage msg)
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

            Send(msg.Channel.Id, embed);
        }

        private async Task SendInvalidFormat(SocketMessage msg)
        {
            await msg.Channel.SendMessageAsync(_invalidFormatMsg);
        }

        private const string _invalidFormatMsg = "Invalid code block.";
    }
}
