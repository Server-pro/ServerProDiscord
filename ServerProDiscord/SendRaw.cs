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

        private static bool _init = false;

        public static void Init()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bot " + Bot.Token);
            _init = true;
        }

        /// <summary>
        /// Sends a sends a custom message using json format. Prints response code if not "OK."
        /// </summary>
        /// <param name="channel">The channel to send the message to.</param>
        /// <param name="json">The formatted json sent in the request. Not altered in the method.</param>
        /// <returns>Returns the HttpResponseMessage from the api.</returns>
        public static void Send(ulong channel, string json)
        {
            if (!_init) Init();

            //build and send request
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var _response = _client.PostAsync($"https://discord.com/api/channels/{channel}/messages", content);

            //log error codes
            if (_response.Result.StatusCode.ToString() != "OK") Console.WriteLine(_response.Result.StatusCode);
        }
    }
}
