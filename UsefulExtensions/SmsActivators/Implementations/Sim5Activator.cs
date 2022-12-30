using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using UsefulExtensions.SmsActivators.Exceptions;
using UsefulExtensions.SmsActivators.Types;

namespace UsefulExtensions.SmsActivators.Implementations
{
	public class Sim5Activator : ISmsActivator
	{
        public string ApiKey { get; set; }

        public Sim5Activator(string apiKey) 
        {
            ApiKey = apiKey; 
        }

        public decimal GetBalance()
        {
            var request = new HttpRequest();
            request.Authorization = $"Bearer {ApiKey}";

            return JObject.Parse(request.Get("https://5sim.net/v1/user/profile").ToString())["balance"].Value<decimal>();
        }

        public async Task<decimal> GetBalanceAsync() => await Task.Run(() => GetBalance());

        public Number GetNumber(string service, string country = null, string @operator = null)
        {
            var request = new HttpRequest();
            request.Authorization = $"Bearer {ApiKey}";
            var response = request.Get($"https://5sim.net/v1/user/buy/activation/{(country == null ? "any" : country)}/{(@operator == null ? "any" : @operator)}/{service}");

            JObject json = JObject.Parse(response.ToString());

            return new Number(json["id"].Value<int>(), json["number"].Value<string>());
        }

        public async Task<Number> GetNumberAsync(string service, string country = null, string @operator = null) => await Task.Run(() => GetNumber(service, country, @operator));

        public SetStatusResult SetStatus(int id, SetStatusEnum status)
        {
            var request = new HttpRequest();
            request.Authorization = $"Bearer {ApiKey}";

            if (status == SetStatusEnum.EndActivation)
            {
                request.Get($"https://5sim.net/v1/user/finish/{id}");
                return SetStatusResult.AccessCancel;
            }
            else if (status == SetStatusEnum.CancelActivation)
            {
                request.Get($"https://5sim.net/v1/user/cancel/{id}");
                return SetStatusResult.AccessCancel;
            }

            return SetStatusResult.AccessCancel;
        }

        public Task<SetStatusResult> SetStatusAsync(int id, SetStatusEnum status) => Task.Run(() => SetStatus(id, status));
       
        public Status GetStatus(int id)
        {
            var request = new HttpRequest();
            request.Authorization = $"Bearer {ApiKey}";
            var response = request.Get($"https://5sim.net/v1/user/check/{id}").ToString();

            JObject json = JObject.Parse(response);

            string status = json["status"].Value<string>();

            if (status == "RECEIVED")
            {
                var lastSms = json["sms"].Value<JToken[]>().Last();

                return new Status(StatusEnum.StatusOk, lastSms["code"]?.Value<string>() == null ? lastSms["text"].Value<string>() : lastSms["code"].Value<string>());
            }
            else if (status == "PENDING")
            {
                return new Status(StatusEnum.StatusWaitCode, null);
            }
            else if (status == "CANCELED" || status == "TIMEOUT" || status == "BANNED")
            {
                return new Status(StatusEnum.StatusCancel, null);
            }
            else if (status == "FINISHED")
            {
                var lastSms = json["sms"].Value<JObject[]>().Last();

                return new Status(StatusEnum.StatusOk, lastSms["code"]?.Value<string>() == null ? lastSms["text"].Value<string>() : lastSms["code"].Value<string>());
            }
            else throw new SmsActivatorException("Unknown status");
        }

        public async Task<Status> GetStatusAsync(int id) => await Task.Run(() => GetStatus(id));
    }
}
