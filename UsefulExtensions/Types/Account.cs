namespace UsefulExtensions
{
    public class Account
    {
        /// <summary>
        /// Возвращает аккаунт с рандомно сгенерированными логином и паролем.
        /// </summary>
        /// <param name="usernameLength">Длина рандомно сгенерированного логина</param>
        /// <param name="passwordLength">Длина рандомно сгенерированного пароля</param>
        /// <returns>Аккаунт с рандомно сгенерированными логином и паролем</returns>
        /// <remarks>
        /// Для генерации используется <see cref="RandomStringGenerator.AllSymbolsGenerator"/>
        /// </remarks>
        public static Account GenerateRandomAccount(int usernameLength, int passwordLength)
        {
            RandomStringGenerator randomString = RandomStringGenerator.AllSymbolsGenerator;
            return new Account(randomString.Generate(usernameLength), randomString.Generate(passwordLength));
        }

        /// <summary>
        /// Конструктор класса <see cref="Account"/>
        /// </summary>
        /// <param name="login">Логин аккаунта</param>
        /// <param name="password">Пароль аккаунта</param>
        public Account(string login, string password)
        {
            Login = login;
            Password = password;
        }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Проверен ли аккаунт. Обычно используется для чекеров.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Возвращает строковое представление аккаунта в формате <c>login:password</c>
        /// </summary>
        /// <returns>Строковое представление аккаунта в формате <c>login:password</c></returns>
        public override string ToString() => ToString(":");

        /// <summary>
        /// Возвращает логин и пароль аккаунта, разделенные указанным разделителем
        /// </summary>
        /// <param name="separator">Разделитель между логином и паролем</param>
        /// <returns>Логин и пароль аккаунта, разделенные указанным разделителем</returns>
        public string ToString(string separator) => $"{Login}{separator}{Password}";
    }
}