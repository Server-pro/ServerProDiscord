using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace ServerProDiscord
{
    class Server
    {
        static HttpClient client;
        static bool IsInit = false;
        public static void Init()
        {
            client = new HttpClient();
            string cookie = System.IO.File.ReadAllText("../../../../cookie.txt");
            client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9,fr;q=0.8,ko;q=0.7");
            client.DefaultRequestHeaders.Add("path", "/api/meta/get");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            client.DefaultRequestHeaders.Add("cookie", $"cookie={cookie}");
        }
        public static Server GetServerFromID(string id)
        {
            if (!IsInit) Init();

            var body = $"id={id}&v=45";
            var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = client.PostAsync("https://server.pro/api/meta/get", content);

            JObject jObject = JsonConvert.DeserializeObject<JObject>(response.Result.Content.ReadAsStringAsync().Result);
            jObject.TryGetValue("servers", out JToken jToken);
            ((JObject)jToken).TryGetValue(id, out JToken jserver);
            return jserver.ToObject<Server>();
        }

        public Conf Conf;
        string Cores;
        bool Down;
        ulong EndTime;
        string FTPPass;
        string Game;
        string Host;
        public ulong ID;
        string IP;
        public string Location;
        string Memory;
        public string Name;
        string Pack;
        string PackNext;
        string Plan;
        int PlanEnabled;
        string PlanNext;
        int PlanTransfer;
        string Port;
        PortPro PortPro;
        int Premium;
        int Renew;
        bool Shared;
        string State;
        string TimeZone;
        uint TTL;
        public string Type;
        string Version;
    }

    class Conf
    {
        string JavaVersion;
        MySql MySql;
        bool PluginsSupported;
        bool SP_AcceptDonations;
        bool SP_ShowInfo;
        bool SP_ShowOnline;
        bool SP_ShowRecent;
    }

    class MySql
    {
        bool Enabled;
        string Password;
    }

    class PortPro
    {
        bool? Updating;
        string? value;
    }
}
