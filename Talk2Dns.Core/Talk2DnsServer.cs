using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ARSoft.Tools.Net.Dns;
using WeatherNet.Clients;

namespace Talk2Dns.Core
{
    public class Talk2DnsServer
    {
        private readonly IPEndPoint endpoint;
        private readonly string domain;
        private DnsServer server;

        public Talk2DnsServer(IPEndPoint endpoint, string domain)
        {
            this.endpoint = endpoint;
            this.domain = domain;

            this.server = new DnsServer(this.endpoint.Address, 10, 10, ProcessQuery);
        }

        private DnsMessageBase ProcessQuery(DnsMessageBase message, IPAddress clientAddress, ProtocolType protocolType)
        {
            message.IsQuery = false;
            DnsMessage query = message as DnsMessage;
            // check for valid query
            if ((query != null) && (query.Questions.Count == 1) && (query.Questions[0].RecordType == RecordType.Txt)) {

                var fqdn = query.Questions[0].Name;

                if (fqdn.EndsWith(this.domain))
                {
                    var subdomain = fqdn.Substring(0, fqdn.Length - this.domain.Length - 1);

                    var splitted = subdomain.Split(new[] {'-'}, 2);

                    var cmd = splitted[0].ToUpper();
                    var payload = splitted.Count() == 2 ? splitted[1].Replace(".", ".").ToLower() : string.Empty;

                    var response = this.ExecuteCommand(cmd, payload);

                    if (response != null)
                    {
                        query.ReturnCode = ReturnCode.NoError;
                        query.AnswerRecords.Add(new TxtRecord(fqdn, 1, response));
                    }
                    else
                    {
                        query.ReturnCode = ReturnCode.NoError;
                        query.AnswerRecords.Add(new TxtRecord(fqdn, 1, "Cannot understand '" + cmd + "'"));
                    }
                }
                else
                {
                    query.ReturnCode = ReturnCode.NoError;
                    query.AnswerRecords.Add(new TxtRecord(fqdn, 1, "DEBUG: You asked: " + fqdn));
                }
            }
            else
            {
                message.ReturnCode = ReturnCode.ServerFailure;
            }
            return message;
        }

        private string ExecuteCommand(string cmd, string payload)
        {
            var response = string.Empty;
            
            switch (cmd)
            {
                case "TIME":
                    response = "UTCnow: " + DateTime.UtcNow.ToLongDateString() + ", " + DateTime.UtcNow.ToLongTimeString();
                    break;

                case "REV":
                    response = new string(payload.Reverse().ToArray());
                    break;

                case "PING":
                    response = payload;
                    break;

                case "WTR":
                    response = this.GetWeatherInfoFor(payload);
                    break;

                case "JS":
                    response = "<script>alert(\"hacked!\");</script>";
                    break;
            }

            return response;
        }

        private string GetWeatherInfoFor(string payload)
        {
            var splitted = payload.Split(new[] {'-'}, 2);

            var response = string.Empty;
            var dayTemplate = "{0}: {1:0}C, {2}, {3:0}-{4:0}C";

            var city = splitted[0];
            var coutry = splitted.Count() == 2 ? splitted[1] : "Switzerland";

            var currentWeather = WeatherNet.Clients.CurrentWeather.GetByCityName(city, coutry, "en", "metric");
            {
                var item = currentWeather.Item;
                if (item == null)
                {
                    return "city not found";
                }

                response = "Weather for: " + currentWeather.Item.City + ", " + currentWeather.Item.Country + "\n";
                response += string.Format(dayTemplate, "Now", item.Temp, item.Description, item.TempMin, item.TempMax);

            }

            var next5Days = WeatherNet.Clients.FiveDaysForecast.GetByCityName(city, coutry, "en", "metric");
            {
                if (next5Days.Items == null)
                {
                    response += "\nNO Forecast!";
                }

                foreach (var item in next5Days.Items.GroupBy(g => g.Date.ToShortDateString()))
                {
                    var single = string.Format(dayTemplate, string.Format("{0:dd.MM}", item.First().Date), item.Average(i => i.Temp), 
                        item.ElementAt(item.Count() / 2).Description, item.Min(i => i.TempMin), item.Max(i => i.TempMax));

                    response += "\n" + single;
                }

            };

            return response;
        }

        public void Start()
        {
            this.server.Start();
        }

        public void Stop()
        {
            this.server.Stop();
        }
    }
}
