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
    /// <summary>
    /// Базовый класс для реализаций совместимых с sms-activate.ru API
    /// </summary>
	public abstract class SmsActivatorApiBase : ISmsActivator
    {
        /// <summary>
        /// Сервисный ключ (apiKey) от сервиса смс активации
        /// </summary>
        public string ApiKey { get; protected set; }

        /// <summary>
        /// Партнерский ключ (softId) от сервиса смс активации
        /// </summary>
        public string SoftId { get; protected set; }

        /// <summary>
        /// Endpoint сервиса api
        /// </summary>
        protected abstract string ApiUrl { get; }

        /// <summary>
        /// Помечает результат как ACCESS или STATUS
        /// </summary>
        protected virtual string[] SuccessResponses => new string[2] { "ACCESS", "STATUS" };

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        /// <param name="apiKey">Сервисный ключ (apiKey) от сервиса смс активации</param>
        /// <param name="softId">Партнерский ключ (softId) от сервиса смс активации</param>
        protected SmsActivatorApiBase(string apiKey, string softId = null)
        {
            ApiKey = apiKey;
            SoftId = softId ?? null;
        }

        /// <summary>
        /// Возвращает баланс на сервисе смс активации
        /// </summary>
        /// <returns>Баланс на сервисе смс активации</returns>
        public virtual decimal GetBalance()
        {
            HttpRequest request = new HttpRequest();
            RequestParams @params = new RequestParams()
            {
                { GetParam("action", "getBalance") },
                { GetParam("api_key", ApiKey) },
            };
            string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

            if(CheckForExceptions(response, out Exception ex))
                throw ex;

            return decimal.Parse(response.Split(':')[1], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Возвращает баланс на сервисе смс активации
        /// </summary>
        /// <returns>Баланс на сервисе смс активации</returns>
        public virtual Task<decimal> GetBalanceAsync() => Task.Run(GetBalance);

        /// <summary>
        /// Бронирует номер
        /// </summary>
        /// <param name="service">Сервис, для которого нужно взять номер</param>
        /// <param name="softId">Ключ партнера (не обязательно)</param>
        /// <param name="country">Страна (необязательно)</param>
        /// <param name="operator">Оператор (необязательно)</param>
        /// <returns>Класс <see cref="Number"/>, содержащий Id номера и сам номер телефона</returns>
        public virtual Number GetNumber(string service, string softId = null, string country = null, string @operator = null)
        {
            HttpRequest request = new HttpRequest();

            RequestParams @params = new RequestParams()
            {
                { GetParam("action", "getNumber") },
                { GetParam("api_key", ApiKey) },
                { GetParam("service", service) }
            };
            if(softId != null)
            {
                @params.Add(GetParam("softId", softId));
            }
            if(country != null)
                @params.Add(GetParam("country", country));
            if(@operator != null)
                @params.Add(GetParam("operator", @operator));

            string response = request.Post(ApiUrl, new FormUrlEncodedContent(@params)).ToString();

            if(CheckForExceptions(response, out Exception ex))
                throw ex;

            string[] data = response.Split(':');
            return new Number(int.Parse(data[1]), data[2]);
        }

        /// <summary>
        /// Бронирует номер
        /// </summary>
        /// <param name="service">Сервис, для которого нужно взять номер</param>
        /// <param name="softId">Ключ партнера (не обязательно)</param>
        /// <param name="country">Страна (необязательно)</param>
        /// <param name="operator">Оператор (необязательно)</param>
        /// <returns>Класс <see cref="Number"/>, содержащий Id номера и сам номер телефона</returns>
        public virtual Task<Number> GetNumberAsync(string service, string softId = null, string country = null, string @operator = null) => Task.Run(() => GetNumber(service, softId, country, @operator));

        /// <summary>
        /// Получает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <returns>Статус номера</returns>
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

            if(CheckForExceptions(response, out Exception ex))
                throw ex;

            StatusEnum statusEnum = StatusEnum.StatusWaitCode;

            if(response.StartsWith("STATUS_WAIT_CODE"))
                statusEnum = StatusEnum.StatusWaitCode;
            else if(response.StartsWith("STATUS_WAIT_RESEND"))
                statusEnum = StatusEnum.StatusWaitCode;
            else if(response.StartsWith("STATUS_WAIT_RETRY"))
                statusEnum = StatusEnum.StatusWaitRetry;
            else if(response.StartsWith("STATUS_CANCEL"))
                statusEnum = StatusEnum.StatusCancel;
            else if(response.StartsWith("STATUS_OK"))
                statusEnum = StatusEnum.StatusOk;

            if(statusEnum == StatusEnum.StatusOk || statusEnum == StatusEnum.StatusWaitRetry)
                return new Status(statusEnum, response.Substring(response.IndexOf(':') + 1));

            return new Status(statusEnum, null);
        }

        /// <summary>
        /// Получает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <returns>Статус номера</returns>
        public virtual Task<Status> GetStatusAsync(int id) => Task.Run(() => GetStatus(id));

        /// <summary>
        /// Устанавливает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <param name="status">Статус, который необходимо установить</param>
        /// <returns>Результат установки статуса</returns>
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

            if(CheckForExceptions(response, out Exception ex))
                throw ex;

            SetStatusResult result = SetStatusResult.AccessReady;

            switch(response)
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

        /// <summary>
        /// Устанавливает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <param name="status">Статус, который необходимо установить</param>
        /// <returns>Результат установки статуса</returns>
        public virtual Task<SetStatusResult> SetStatusAsync(int id, SetStatusEnum status) => Task.Run(() => SetStatus(id, status));

        private bool CheckForExceptions(string response, out Exception ex)
        {
            if(!SuccessResponses.Any(response.StartsWith))
            {
                if(response.StartsWith("BAD_KEY"))
                {
                    ex = new SmsBadKeyException();
                }
                if(response.StartsWith("BAD_ACTION"))
                {
                    ex = new SmsBadKeyException();
                }
                if(response.StartsWith("NO_NUMBERS"))
                {
                    ex = new SmsNoNumbersException();
                }
                if(response.StartsWith("NO_BALANCE"))
                {
                    ex = new SmsNoBalanceException();
                }
                ex = new SmsActivatorException(response);
                return true;
            } else
            {
                ex = null;
                return false;
            }
        }

        private static KeyValuePair<string, string> GetParam(string name, string value)
        {
            return new KeyValuePair<string, string>(name, value);
        }
    }
}