using System.Collections.Generic;

namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Абстрактный класс для соотношения доменов эл. почты и IMAP серверов
    /// </summary>
    public abstract class ImapServerSelector : IImapServerSelector
    {
        /// <summary>
        /// Словарь домен - сервер
        /// </summary>
        public Dictionary<string, ImapServer> Servers { get; protected set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ImapServerSelector()
        {
            Servers = new Dictionary<string, ImapServer>();
        }

        /// <summary>
        /// Получает сервер для заданного домена эл. почты
        /// </summary>
        /// <param name="domain">Домен эл. почты</param>
        /// <returns></returns>
        public ImapServer GetServer(string domain)
        {
            foreach (var server in Servers)
            {
                if (domain.ToLower() == server.Key.ToLower())
                    return server.Value;
            }

            return null;
        }
    }
}
