using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerProDiscord
{
    public static class SendRaw
    {
        private static HttpClient _client;
        private static Task<HttpResponseMessage> _response;

        public static void Init()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bot " + Program.Token);
        }

        /// <summary>
        /// Sends a sends a custom message using json format. Prints response code if not "OK."
        /// </summary>
        /// <param name="channel">The channel to send the message to.</param>
        /// <param name="json">The formatted json sent in the request. Not altered in the method.</param>
        /// <returns>Returns the HttpResponseMessage from the api.</returns>
        public static Task<HttpResponseMessage> Send(ulong channel, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _response = _client.PostAsync($"https://discord.com/api/channels/{channel}/messages", content);
            if (_response.Result.StatusCode.ToString() != "OK") Console.WriteLine(_response.Result.StatusCode);
            return _response;
        }
    }
}
