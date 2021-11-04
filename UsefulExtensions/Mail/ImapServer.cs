namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Класс данных об IMAP сервере
    /// </summary>
    public class ImapServer
    {
        /// <summary>
        /// Хост IMAP сервера
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// Порт IMAP сервера
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Конструктор <see cref="ImapServer"/>
        /// </summary>
        /// <param name="host">Хост</param>
        /// <param name="port">Порт</param>
        public ImapServer(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
