namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Перечисление результатов проверки почты на валид
    /// </summary>
    public enum IsValidResult
    {
        /// <summary>
        /// Почта валид, проблем с подключением и авторизацией не возникло
        /// </summary>
        Valid,

        /// <summary>
        /// Проблема с соединением. Если не используется прокси, то, скорее всего, неверный imap-сервер
        /// </summary>
        ConnectionError,

        /// <summary>
        /// Неправильный логин или пароль
        /// </summary>
        AuthError
    }
}
