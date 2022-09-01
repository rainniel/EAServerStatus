using EAServerStatus.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EAServerStatus.Utils
{
    public static class EAWebClient
    {
        private static readonly HttpClient Client;
        private const string URL = "https://www.euroaion.com/en-US";
        private const int MAX_READLINE = 110;

        static EAWebClient()
        {
            Client = new HttpClient();
        }

        private static string GetPageSource()
        {
            try
            {
                using (HttpResponseMessage response = Client.GetAsync(URL).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ServerStatus> GetServerStatus()
        {
            return await Task.Run(() =>
            {
                var pageSource = GetPageSource();

                if (pageSource == null)
                {
                    return new ServerStatus(Status.RequestError);
                }

                int online = 0;
                int elyos = 0;
                int asmo = 0;
                Status status = Status.Offline;

                string[] lines = pageSource.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                int loop = 0;
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.Equals("<title>The network path was not found</title>", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ServerStatus(Status.ServerError);
                    }
                    else if (trimmedLine.StartsWith("Status:ONLINE", StringComparison.OrdinalIgnoreCase))
                    {
                        online = trimmedLine.GetSplitIndex(' ', 1).ToInt();
                        status = online > 0 ? Status.Online : Status.ZeroPlayer;
                    }
                    else if (trimmedLine.StartsWith("(Elyos:", StringComparison.OrdinalIgnoreCase))
                    {
                        trimmedLine = trimmedLine.Replace(" ", "");
                        trimmedLine = trimmedLine.Replace("(", "");
                        trimmedLine = trimmedLine.Replace(")", "");
                        trimmedLine = trimmedLine.Replace("%", "");

                        var percentage = trimmedLine.Split(',');
                        if (percentage.Length == 2)
                        {
                            elyos = percentage[0].GetSplitIndex(':', 1).ToInt();
                            asmo = percentage[1].GetSplitIndex(':', 1).ToInt();
                        }

                        break;
                    }

                    loop++;
                    if (loop >= MAX_READLINE) return new ServerStatus(Status.DataError);
                }

                return new ServerStatus(online, elyos, asmo, status);
            });
        }
    }
}
