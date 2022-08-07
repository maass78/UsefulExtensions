using System.Threading.Tasks;
using UsefulExtensions.SmsActivators.Types;

namespace UsefulExtensions.SmsActivators
{
    public interface ISmsActivator
    {
        /// <summary>
        /// Сервисный ключ (apiKey) от сервиса смс активации
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Партнерский ключ (softId) от сервиса смс активации
        /// </summary>
        string SoftId { get; }

        /// <summary>
        /// Возвращает баланс на сервисе смс активации
        /// </summary>
        /// <returns>Баланс на сервисе смс активации</returns>
        decimal GetBalance();

        /// <summary>
        /// Возвращает баланс на сервисе смс активации
        /// </summary>
        /// <returns>Баланс на сервисе смс активации</returns>
        Task<decimal> GetBalanceAsync();

        /// <summary>
        /// Бронирует номер
        /// </summary>
        /// <param name="service">Сервис, для которого нужно взять номер</param>
        /// <param name="softId">Ключ партнера (не обязательно)</param>
        /// <param name="country">Страна (необязательно)</param>
        /// <param name="operator">Оператор (необязательно)</param>
        /// <returns>Класс <see cref="Number"/>, содержащий Id номера и сам номер телефона</returns>
        Number GetNumber(string service, string softId = null, string country = null, string @operator = null);

        /// <summary>
        /// Бронирует номер
        /// </summary>
        /// <param name="service">Сервис, для которого нужно взять номер</param>
        /// <param name="softId">Ключ партнера (не обязательно)</param>
        /// <param name="country">Страна (необязательно)</param>
        /// <param name="operator">Оператор (необязательно)</param>
        /// <returns>Класс <see cref="Number"/>, содержащий Id номера и сам номер телефона</returns>
        Task<Number> GetNumberAsync(string service, string softId = null, string country = null, string @operator = null);

        /// <summary>
        /// Устанавливает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <param name="status">Статус, который необходимо установить</param>
        /// <returns>Результат установки статуса</returns>
        SetStatusResult SetStatus(int id, SetStatusEnum status);

        /// <summary>
        /// Устанавливает статус номера
        /// </summary>
        /// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
        /// <param name="status">Статус, который необходимо установить</param>
        /// <returns>Результат установки статуса</returns>
        Task<SetStatusResult> SetStatusAsync(int id, SetStatusEnum status);

        /// <summary>
		/// Получает статус номера
		/// </summary>
		/// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
		/// <returns>Статус номера</returns>
        Status GetStatus(int id);

        /// <summary>
		/// Получает статус номера
		/// </summary>
		/// <param name="id">Id номера, содержится в экземпляре класса <see cref="Number"/></param>
		/// <returns>Статус номера</returns>
        Task<Status> GetStatusAsync(int id);
    }
}