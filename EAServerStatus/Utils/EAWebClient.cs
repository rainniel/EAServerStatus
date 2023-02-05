using EAServerStatus.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EAServerStatus.Utils
{
    public static class EAWebClient
    {
        private const string Url = "https://www.euroaion.com/en-US";
        private const int MaxReadLine = 110;

        private static readonly HttpClient _client = new HttpClient();

        private static string GetPageSource()
        {
            try
            {
                using (HttpResponseMessage response = _client.GetAsync(Url).Result)
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

                var online = 0;
                var elyos = 0;
                var asmo = 0;
                Status status = Status.Null;

                var lines = pageSource.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();

                    if (online == 0 && line.StartsWith("Status:ONLINE", StringComparison.OrdinalIgnoreCase))
                    {
                        online = line.GetSplitIndex(' ', 1).ToInt();
                        status = online > 0 ? Status.Online : Status.ZeroPlayer;
                    }
                    else if (line.StartsWith("(Elyos:", StringComparison.OrdinalIgnoreCase))
                    {
                        line = line.Replace(" ", "");
                        line = line.Replace("(", "");
                        line = line.Replace(")", "");
                        line = line.Replace("%", "");

                        var percentage = line.Split(',');
                        if (percentage.Length == 2)
                        {
                            elyos = percentage[0].GetSplitIndex(':', 1).ToInt();
                            asmo = percentage[1].GetSplitIndex(':', 1).ToInt();
                        }

                        break;
                    }
                    else if (line.Equals("<i class=\"fa fa-dot-circle-o\" style=\"color: red\">", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ServerStatus(Status.Maintenance);
                    }
                    else if (line.StartsWith("<span><H1>Server Error in '", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ServerStatus(Status.WebsiteError);
                    }

                    if (i >= MaxReadLine) return new ServerStatus(Status.DataError);
                }

                return new ServerStatus(online, elyos, asmo, status);
            });
        }
    }
}
