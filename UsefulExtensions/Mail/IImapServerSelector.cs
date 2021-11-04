namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Интерфейс для получения imap сервера по домену эл. почты
    /// </summary>
    public interface IImapServerSelector
    {
        /// <summary>
        /// Получает сервер для заданного домена эл. почты
        /// </summary>
        /// <param name="domain">Домен эл. почты</param>
        /// <returns></returns>
        ImapServer GetServer(string domain);
    }
}
