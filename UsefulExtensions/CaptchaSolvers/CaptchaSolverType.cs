using UsefulExtensions.CaptchaSolvers.Implementations;

namespace UsefulExtensions.CaptchaSolvers
{
    /// <summary>
    /// Тип решателя капчи. Обычно используется для выбора сервиса решения капчи в графическом интерфейсе.
    /// </summary>
    public enum CaptchaSolverType
    {
        /// <summary>
        /// https://rucaptcha.com/
        /// </summary>
        Rucaptcha,
        /// <summary>
        /// https://anti-captcha.com/
        /// </summary>
        AntiCaptcha,
        /// <summary>
        /// https://capmonster.cloud/ru/
        /// </summary>
        CapMonsterCloud
    }

    /// <summary>
    /// Предоставляет методы расширения для перечисления <see cref="CaptchaSolverType"/>
    /// </summary>
    public static class CaptchaSolverTypeExtensions 
    {
        /// <summary>
        /// Получение <see cref="ICaptchaSolver"/> по типу используемого сервиса
        /// </summary>
        /// <param name="type">Сервис решения капчи</param>
        /// <param name="apiKey">Сервисный ключ (api key) к указанному сервису решения капчи</param>
        /// <returns>Одна из стандартных реализаций интерфейса <see cref="ICaptchaSolver"/> в зависимости от указанного типа</returns>
        public static ICaptchaSolver GetCaptchaSolverByType(this CaptchaSolverType type, string apiKey)
        {
            switch (type)
            {
                case CaptchaSolverType.Rucaptcha:
                    return new RucaptchaSolver(apiKey);

                case CaptchaSolverType.AntiCaptcha:
                    return new AnticaptchaSolver(apiKey);

                case CaptchaSolverType.CapMonsterCloud:
                    return new CapmonsterCloudSolver(apiKey);
            }

            return null;
        }
    }
}
