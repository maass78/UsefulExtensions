namespace UsefulExtensions.CaptchaSolvers.Models
{
    public class HCaptchaResult
    {
        /// <summary>
        /// Строка токена, которая требуется для отправки формы на целевом сайте.
        /// </summary>
        public string GRecaptchaResponse { get; set; }

        /// <summary>
        /// Результат функции "window.hcaptcha.getRespKey()" когда она доступна. Некоторые сайты используют это значение для дополнительных проверок.
        /// </summary>
        public string RespKey { get; set; }

        /// <summary>
        /// User-Agent браузера работника. Используйте его когда отправляете форму с токеном.
        /// </summary>
        public string UserAgent { get; set; }
    }
}
