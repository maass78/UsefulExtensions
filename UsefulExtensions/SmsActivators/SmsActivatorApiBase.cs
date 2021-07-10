using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Leaf.xNet;
using UsefulExtensions.SmsActivators.Exceptions;
using UsefulExtensions.SmsActivators.Types;

namespace UsefulExtensions.SmsActivators
{
    public abstract class SmsActivatorApiBase : ISmsActivator
	{
		public string ApiKey { get; protected set; }
		protected abstract string ApiUrl { get; }
		protected virtual string[] SuccessResponses => new string[2] { "ACCESS", "STATUS" };

		public SmsActivatorApiBase(string apiKey)
		{
			ApiKey = apiKey;
		}

		public virtual decimal GetBalance()
        {
			HttpRequest request = new HttpRequest();
			RequestParams @params = new RequestParams()
			{
				{ GetParam("action", "getBalance") },
				{ GetParam("api_key", ApiKey) },
			};
			string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

			if (CheckForExceptions(response, out Exception ex))
				throw ex;

			return decimal.Parse(response.Split(':')[1], CultureInfo.InvariantCulture);
        }
		public virtual async Task<decimal> GetBalanceAsync() => await Task.Run(() => GetBalance());

		public virtual Number GetNumber(string service, string country = null, string @operator = null)
        {
			HttpRequest request = new HttpRequest();
			RequestParams @params = new RequestParams()
			{
				{ GetParam("action", "getNumber") },
				{ GetParam("api_key", ApiKey) },
				{ GetParam("service", service) },
			};
			if (country != null)
				@params.Add(GetParam("country", country));
			if (@operator != null)
				@params.Add(GetParam("operator", @operator));

			string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

			if (CheckForExceptions(response, out Exception ex))
				throw ex;

			string[] data = response.Split(':');
			return new Number(int.Parse(data[1]), data[2]);
		}
		public virtual async Task<Number> GetNumberAsync(string service, string country = null, string @operator = null) => await Task.Run(() => GetNumber(service, country, @operator));

		public virtual Status GetStatus(int id)
        {
			HttpRequest request = new HttpRequest();
			RequestParams @params = new RequestParams()
			{
				{ GetParam("action", "getStatus") },
				{ GetParam("api_key", ApiKey) },
				{ GetParam("id", id.ToString()) },
			};
			string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

			if (CheckForExceptions(response, out Exception ex))
				throw ex;

			StatusEnum statusEnum = StatusEnum.StatusWaitCode;

			if (response.StartsWith("STATUS_WAIT_CODE"))
				statusEnum = StatusEnum.StatusWaitCode;
			else if (response.StartsWith("STATUS_WAIT_RESEND"))
				statusEnum = StatusEnum.StatusWaitCode;
			else if (response.StartsWith("STATUS_WAIT_RETRY"))
				statusEnum = StatusEnum.StatusWaitRetry;
			else if (response.StartsWith("STATUS_CANCEL"))
				statusEnum = StatusEnum.StatusCancel;
			else if (response.StartsWith("STATUS_OK"))
				statusEnum = StatusEnum.StatusOk;

			if (statusEnum == StatusEnum.StatusOk || statusEnum == StatusEnum.StatusWaitRetry)
				return new Status(statusEnum, response.Substring(response.IndexOf(':') + 1));

			return new Status(statusEnum, null);
		}
		public virtual async Task<Status> GetStatusAsync(int id) => await Task.Run(() => GetStatus(id));

		public virtual SetStatusResult SetStatus(int id, SetStatusEnum status)
        {
			HttpRequest request = new HttpRequest();
			RequestParams @params = new RequestParams()
			{
				{ GetParam("action", "setStatus") },
				{ GetParam("api_key", ApiKey) },
				{ GetParam("id", id.ToString()) },
				{ GetParam("status", ((int)status).ToString()) }
			};

			string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

			if (CheckForExceptions(response, out Exception ex))
				throw ex;

			SetStatusResult result = SetStatusResult.AccessReady;

			switch (response)
			{
				case "ACCESS_READY":
					result = SetStatusResult.AccessReady;
					break;
				case "ACCESS_RETRY_GET":
					result = SetStatusResult.AccessReadyGet;
					break;
				case "ACCESS_ACTIVATION":
					result = SetStatusResult.AccessActivation;
					break;
				case "ACCESS_CANCEL":
					result = SetStatusResult.AccessCancel;
					break;
			}
			return result;
		}
		public virtual async Task<SetStatusResult> SetStatusAsync(int id, SetStatusEnum status) => await Task.Run(() => SetStatus(id, status));

		private bool CheckForExceptions(string response, out Exception ex)
		{
			if (!SuccessResponses.Any((string s) => response.StartsWith(s)))
			{
				if (response.StartsWith("BAD_KEY"))
				{
					ex = new SmsBadKeyException();
				}
				if (response.StartsWith("BAD_ACTION"))
				{
					ex = new SmsBadKeyException();
				}
				if (response.StartsWith("NO_NUMBERS"))
				{
					ex = new SmsNoNumbersException();
				}
				if (response.StartsWith("NO_BALANCE"))
				{
					ex = new SmsNoBalanceException();
				}
				ex = new SmsActivatorException(response);
				return true;
			}
            else
            {
				ex = null;
				return false;
            }
		}
		private KeyValuePair<string, string> GetParam(string name, string value)
        {
			return new KeyValuePair<string, string>(name, value);
        }
	}
}
