using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        public void SendRaw(ulong channel, string json)
        {
            //build and send request
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var _response = _client.PostAsync($"https://discord.com/api/channels/{channel}/messages", content);

            //log error codes
            if (_response.Result.StatusCode.ToString() != "OK") Console.WriteLine(_response.Result.StatusCode);
        }

        public bool StripCodeBlock(string content, out string result)
        {
            result = null;
            Regex codeBlock = new Regex("```[^`]*```", RegexOptions.Multiline);
            var match = codeBlock.Match(content);

            if (match.Success) result = match.Value.Substring(3, match.Length - 6); ;

            return match.Success;
        }
    }
}
