namespace UsefulExtensions.CaptchaSolvers.Implementations
{
    /// <summary>
    /// еализация интерфейса <see cref="ICaptchaSolver"/> для сервиса CaptchaGuru (<see href="https://rucaptcha.com/"/>)
    /// </summary>
    public class CaptchaGuruSolver : ApiCaptchaSolver
    {
        /// <summary>
        /// Констурктор класса <see cref="CaptchaGuruSolver"/>
        /// </summary>
        /// <param name="apiKey">API ключ для доступа к сервису</param>
        public CaptchaGuruSolver(string apiKey) : base("https://api.captcha.guru/", apiKey)
        {
        }
    }
}
