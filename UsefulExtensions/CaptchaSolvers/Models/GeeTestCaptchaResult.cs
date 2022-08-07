namespace UsefulExtensions.CaptchaSolvers.Models
{
    /// <summary>
    /// Класс, содержащий информацию об решении капчи geetest
    /// </summary>
    public class GeeTestV3CaptchaResult
    {
        /// <summary>
        /// Строка-хэш, требуется для взаимодействия с формой на целевом сайте
        /// </summary>
        public string Challenge { get; set; }

        /// <summary>
        /// Строка-хэш, тоже требуется.
        /// </summary>
        public string Validate { get; set; }

        /// <summary>
        /// Еще одна строка, мы без понятия зачем их 3 штуки.
        /// </summary>
        public string Seccode { get; set; }
    }

    public class GeeTestV4CaptchaResult
    {
        public string CaptchaId { get; set; }

        public string LotNumber { get; set; }

        public string PassToken { get; set; }

        public int GenTime { get; set; }

        public string CaptchaOutput { get; set; }
    }
}