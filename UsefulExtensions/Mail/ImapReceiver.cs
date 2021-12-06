using MailKit.Net.Imap;
using MailKit.Net.Proxy;
using MailKit.Search;
using MimeKit;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Класс, используемый для быстрого и удобного получения писем с электронной почты
    /// </summary>
    public class ImapReceiver
    {
        /// <summary>
        /// Задержка между проверкой почтового ящика в методах <see cref="WaitLastMail(string, string, ProxyClient, string, CancellationToken)"/>
        /// </summary>
        public static int Delay { get; set; } = 2000;

        /// <summary>
        /// Реализация <see cref="IImapServerSelector"/>, с помощью которой домен эл. почты соотносится с IMAP сервером
        /// <br/>Значение по умолчанию - <see cref="DefaultImapServerSelector"/>
        /// </summary>
        public static IImapServerSelector ServerSelector { get; set; } = new DefaultImapServerSelector();

        /// <summary>
        /// Проверяет почту на действительность
        /// </summary>
        /// <param name="mail">Адрес эл. почты</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <returns><c>true</c>, если почта действительна, иначе <c>false</c></returns>
        public static IsValidResult IsValid(string mail, string password, ProxyClient proxy)
        {
            if(ServerSelector == null)
            {
                throw new InvalidOperationException("ServerSelector был null");
            }

            try
            {
                ImapServer server = ServerSelector.GetServer(mail.Split('@').Last());

                ImapClient client = new ImapClient() 
                {
                    ProxyClient = proxy
                };

                try
                {
                    client.Connect(server.Host, server.Port);
                }
                catch
                {
                    return IsValidResult.ConnectionError;
                }

                try
                {
                    client.Authenticate(mail, password);
                }
                catch
                {
                    return IsValidResult.AuthError;
                }

                return IsValidResult.Valid;
            }
            catch
            {
                return IsValidResult.ConnectionError;
            }
        }

        /// <summary>
        /// Получает последнее письмо с эл. почты
        /// <br/> Если параметр <c>from</c> задан, то возвращается последнее письмо от заданного отправителя
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="from">Адрес отправителя</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static MimeMessage GetLastMail(string mail, string password, ProxyClient proxy, string from = null)
        {
            ImapServer server = ServerSelector.GetServer(mail.Split('@').Last());

            ImapClient client = new ImapClient()
            {
                ProxyClient = proxy
            };
            client.Connect(server.Host, server.Port);
            client.Authenticate(mail, password);
            var inbox = client.Inbox;
            inbox.Open(MailKit.FolderAccess.ReadOnly);
            var results = inbox.Search(SearchQuery.All).ToList();
            results.Reverse();
            foreach (var uniqueId in results)
            {
                var message = inbox.GetMessage(uniqueId);

                if (from != null)
                {
                    string address = Regex.Match(message.From.FirstOrDefault().ToString(), @"\<(.*?)\>").Groups[1].Value;

                    if (address == from)
                    {
                        return message;
                    }
                }
                else return message;
            }

            return null;
        }

        /// <summary>
        /// Получает последнее письмо с эл. почты
        /// <br/> Если параметр <c>predicate</c> задан, то возвращается последнее письмо, удовлетворяющее условию <see cref="Predicate{MimeMessage}"/>
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="predicate">Условие, которому должно соответствовать письмо</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static MimeMessage GetLastMail(string mail, string password, ProxyClient proxy, Predicate<MimeMessage> predicate = null)
        {
            ImapServer server = ServerSelector.GetServer(mail.Split('@').Last());

            ImapClient client = new ImapClient()
            {
                ProxyClient = proxy
            };
            client.Connect(server.Host, server.Port);
            client.Authenticate(mail, password);
            var inbox = client.Inbox;
            inbox.Open(MailKit.FolderAccess.ReadOnly);
            var results = inbox.Search(SearchQuery.All).ToList();
            results.Reverse();
            foreach (var uniqueId in results)
            {
                var message = inbox.GetMessage(uniqueId);

                if (predicate != null)
                {
                    if (predicate(message))
                    {
                        return message;
                    }
                }
                else return message;
            }

            return null;
        }

        /// <summary>
        /// Асинхронно получает последнее письмо с эл. почты
        /// <br/> Если параметр <c>from</c> задан, то возвращается последнее письмо от заданного отправителя
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="from">Адрес отправителя</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static async Task<MimeMessage> GetLastMailAsync(string mail, string password, ProxyClient proxy, string from = null) 
            => await Task.Run(() => GetLastMail(mail, password, proxy, from));

        /// <summary>
        /// Асинхронно получает последнее письмо с эл. почты
        /// <br/> Если параметр <c>predicate</c> задан, то возвращается последнее письмо, удовлетворяющее условию <see cref="Predicate{MimeMessage}"/>
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="predicate">Условие, которому должно соответствовать письмо</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static async Task<MimeMessage> GetLastMailAsync(string mail, string password, ProxyClient proxy, Predicate<MimeMessage> predicate = null)
            => await Task.Run(() => GetLastMail(mail, password, proxy, predicate));

        /// <summary>
        /// Получает последнее письмо с эл. почты. Если такого письма нет, ожидает его получения. Интервал проверки задается свойством <see cref="Delay"/>
        /// <br/> Если параметр <c>from</c> задан, то возвращается последнее письмо от заданного отправителя
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="token">Токен для отмены</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static MimeMessage WaitLastMail(string mail, string password, ProxyClient proxy, string from = null, CancellationToken token = default)
        {
            MimeMessage message = GetLastMail(mail, password, proxy, from);

            while(message == null && !token.IsCancellationRequested)
            {
                message = GetLastMail(mail, password, proxy, from);
                Thread.Sleep(Delay);
            }

            return message;
        }
        /// <summary>
        /// Асинхронного получает последнее письмо с эл. почты. Если такого письма нет, ожидает его получения. Интервал проверки задается свойством <see cref="Delay"/>
        /// <br/> Если параметр <c>from</c> задан, то возвращается последнее письмо от заданного отправителя
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="from">Адрес отправителя</param>
        /// <param name="token">Токен для отмены</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static async Task<MimeMessage> WaitLastMailAsync(string mail, string password, ProxyClient proxy, string from = null, CancellationToken token = default)
            => await Task.Run(() => WaitLastMail(mail, password, proxy, from, token));

        /// <summary>
        /// Получает последнее письмо с эл. почты, Если такого письма нет, ожидает его получения. Интервал проверки задается свойством <see cref="Delay"/>
        /// <br/> Если параметр <c>predicate</c> задан, то возвращается последнее письмо, удовлетворяющее условию
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="predicate">Условие, которому должно соответствовать письмо</param>
        /// <param name="token">Токен для отмены</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static MimeMessage WaitLastMail(string mail, string password, ProxyClient proxy, Predicate<MimeMessage> predicate = null, CancellationToken token = default)
        {
            MimeMessage message = GetLastMail(mail, password, proxy, predicate);

            while (message == null && !token.IsCancellationRequested)
            {
                message = GetLastMail(mail, password, proxy, predicate);
                Thread.Sleep(Delay);
            }

            return message;
        }

        /// <summary>
        /// Асинхронного получает последнее письмо с эл. почты. Если такого письма нет, ожидает его получения. Интервал проверки задается свойством <see cref="Delay"/>
        /// <br/> Если параметр <c>from</c> задан, то возвращается последнее письмо, удовлетворяющее условию
        /// </summary>
        /// <param name="mail">Эл. почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="proxy">Прокси</param>
        /// <param name="predicate">Условие, которому должно соответствовать письмо</param>
        /// <param name="token">Токен для отмены</param>
        /// <returns>Письмо, удовлетворяющее условию</returns>
        public static async Task<MimeMessage> WaitLastMailAsync(string mail, string password, ProxyClient proxy, Predicate<MimeMessage> predicate = null, CancellationToken token = default)
            => await Task.Run(() => WaitLastMail(mail, password, proxy, predicate, token));
    }
}
