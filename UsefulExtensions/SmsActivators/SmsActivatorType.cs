using UsefulExtensions.SmsActivators.Implementations;

namespace UsefulExtensions.SmsActivators
{
    /// <summary>
    /// Тип сервиса смс активации. Обычно используется для выбора сервиса смс активации в графическом интерфейсе.
    /// </summary>
    public enum SmsActivatorType
    {
        /// <summary>
        /// https://smshub.org/
        /// </summary>
        SmsHub,
        /// <summary>
        /// https://sms-activate.ru/
        /// </summary>
        SmsActivateRu,
        /// <summary>
        /// https://5sim.net/
        /// </summary>
        Sim5,
        /// <summary>
        /// https://vak-sms.com/
        /// </summary>
        VakSmsCom,
        /// <summary>
        /// https://onlinesim.ru/
        /// </summary>
        OnlineSim,
        /// <summary>
        /// https://sms-actiwator.ru/
        /// </summary>
        SmsActiwator
    }

    /// <summary>
    /// Предоставляет методы расширения для перечисления <see cref="SmsActivatorType"/>
    /// </summary>
    public static class SmsActivatorTypeExtensions
    {
        /// <summary>
        /// Получение <see cref="ISmsActivator"/> по типу используемого сервиса
        /// </summary>
        /// <param name="type">Сервис смс активации</param>
        /// <param name="apiKey">Сервисный ключ (api key) к указанному сервису смс активации</param>
        /// <returns>Одна из стандартных реализаций интерфейса <see cref="ISmsActivator"/> в зависимости от указанного типа</returns>
        public static ISmsActivator GetSmsActivatorByType(this SmsActivatorType type, string apiKey)
        {
            if (type == SmsActivatorType.SmsHub)
                return new SmsHubActivator(apiKey);
            else if (type == SmsActivatorType.SmsActivateRu)
                return new SmsActivateRuActivator(apiKey);
            else if (type == SmsActivatorType.Sim5)
                return new Sim5Activator(apiKey);
            else if (type == SmsActivatorType.OnlineSim)
                return new OnlineSimActivator(apiKey);
            else if (type == SmsActivatorType.SmsActiwator)
                return new SmsActiwatorActivator(apiKey);
            else
                return new VakSmsComActivator(apiKey);
        }
    }
}
