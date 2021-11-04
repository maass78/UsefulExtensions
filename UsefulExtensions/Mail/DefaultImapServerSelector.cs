using System.Collections.Generic;

namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Реализация <see cref="IImapServerSelector"/> по умолчанию
    /// </summary>
    public class DefaultImapServerSelector : ImapServerSelector
    {
        /// <summary>
        /// Конструктор <see cref="DefaultImapServerSelector"/>, где определены домены и сервера по умолчанию:
        /// <code>
        /// <br/>
        /// <br/>mail.ru, inbox.ru, internet.ru, bk.ru, list.ru => imap.mail.ru:993
        /// <br/>hotmail.com, outlook.com => outlook.office365.com:993
        /// <br/>gmail.com => imap.gmail.com:993
        /// <br/>rambler.ru => imap.rambler.ru:993
        /// <br/>yandex.ru => imap.yandex.ru:993
        /// <br/>yahoo.com => imap.mail.yahoo.com:993
        /// <br/>
        /// </code>
        /// </summary>
        public DefaultImapServerSelector()
        {
            Servers.Clear();
            Servers.Add("mail.ru", new ImapServer("imap.mail.ru", 993));
            Servers.Add("inbox.ru", new ImapServer("imap.mail.ru", 993));
            Servers.Add("internet.ru", new ImapServer("imap.mail.ru", 993));
            Servers.Add("bk.ru", new ImapServer("imap.mail.ru", 993));
            Servers.Add("list.ru", new ImapServer("imap.mail.ru", 993));

            Servers.Add("hotmail.com", new ImapServer("outlook.office365.com", 993));
            Servers.Add("outlook.com", new ImapServer("outlook.office365.com", 993));

            Servers.Add("gmail.com", new ImapServer("imap.gmail.com", 993));

            Servers.Add("rambler.ru", new ImapServer("imap.rambler.ru", 993));

            Servers.Add("yandex.ru", new ImapServer("imap.yandex.ru", 993));

            Servers.Add("yahoo.com", new ImapServer("imap.mail.yahoo.com", 993));
        }

        /// <summary>
        /// Конструктор <see cref="DefaultImapServerSelector"/> с пользовательскими серверами
        /// </summary>
        /// <param name="servers">Словарь <see cref="string"/> <see cref="ImapServer"/>, где определены домена и связанные с ним IMAP сервера</param>
        public DefaultImapServerSelector(Dictionary<string, ImapServer> servers)
        {
            Servers = servers;
        }
    }
}
