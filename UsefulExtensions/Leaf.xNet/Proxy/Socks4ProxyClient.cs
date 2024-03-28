using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет клиент для Socks4 прокси-сервера.
    /// </summary>
    public class Socks4ProxyClient : ProxyClient
    {
        #region Константы (защищённые)

        protected const int DefaultPort = 1080;

        protected const byte VersionNumber = 4;

        private const byte CommandConnect = 0x01;
        // protected const byte CommandBind = 0x02;
        protected const byte CommandReplyRequestGranted = 0x5a;
        private const byte CommandReplyRequestRejectedOrFailed = 0x5b;
        private const byte CommandReplyRequestRejectedCannotConnectToIdentd = 0x5c;
        private const byte CommandReplyRequestRejectedDifferentIdentd = 0x5d;

        #endregion


        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4ProxyClient" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public Socks4ProxyClient()
            : this(null) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4ProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        public Socks4ProxyClient(string host, int port = DefaultPort)
            : this(host, port, string.Empty) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4ProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        /// <param name="username">Имя пользователя для авторизации на прокси-сервере.</param>
        public Socks4ProxyClient(string host, int port, string username)
            : base(ProxyType.Socks4, host, port, username, null) { }

        #endregion


        #region Статические методы (закрытые)

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks4ProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <returns>Экземпляр класса <see cref="Socks4ProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="proxyAddress"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="proxyAddress"/> является пустой строкой.</exception>
        /// <exception cref="System.FormatException">Формат порта является неправильным.</exception>
        // ReSharper disable once UnusedMember.Global
        public new static Socks4ProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.Socks4, proxyAddress) as Socks4ProxyClient;
        }

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks4ProxyClient"/>. Возвращает значение, указывающее, успешно ли выполнено преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <param name="result">Если преобразование выполнено успешно, то содержит экземпляр класса <see cref="Socks4ProxyClient"/>, иначе <see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешно, иначе <see langword="false"/>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse(string proxyAddress, out Socks4ProxyClient result)
        {
            if (!TryParse(ProxyType.Socks4, proxyAddress, out var proxy))
            {
                result = null;
                return false;    
            }

            result = proxy as Socks4ProxyClient;
            return true;
        }

        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Создаёт соединение с сервером через прокси-сервер.
        /// </summary>
        /// <param name="destinationHost">Хост сервера, с которым нужно связаться через прокси-сервер.</param>
        /// <param name="destinationPort">Порт сервера, с которым нужно связаться через прокси-сервер.</param>
        /// <param name="tcpClient">Соединение, через которое нужно работать, или значение <see langword="null" />.</param>
        /// <returns>Соединение с сервером через прокси-сервер.</returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// Значение свойства <see cref="!:Host" /> равно <see langword="null" /> или имеет нулевую длину.
        /// -или-
        /// Значение свойства <see cref="!:Port" /> меньше 1 или больше 65535.
        /// -или-
        /// Значение свойства <see cref="!:Username" /> имеет длину более 255 символов.
        /// -или-
        /// Значение свойства <see cref="!:Password" /> имеет длину более 255 символов.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="destinationHost" /> равно <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">Значение параметра <paramref name="destinationHost" /> является пустой строкой.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="destinationPort" /> меньше 1 или больше 65535.</exception>
        /// <exception cref="!:Leaf.xNet.Net.ProxyException">Ошибка при работе с прокси-сервером.</exception>
        public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
        {
            CheckState();

            #region Проверка параметров

            if (destinationHost == null)
                throw new ArgumentNullException(nameof(destinationHost));

            if (destinationHost.Length == 0)
                throw ExceptionHelper.EmptyString(nameof(destinationHost));

            if (!ExceptionHelper.ValidateTcpPort(destinationPort))
                throw ExceptionHelper.WrongTcpPort(nameof(destinationHost));

            #endregion

            var curTcpClient = tcpClient ?? CreateConnectionToProxy();

            try
            {
                SendCommand(curTcpClient.GetStream(), CommandConnect, destinationHost, destinationPort);
            }
            catch (Exception ex)
            {
                curTcpClient.Close();

                if (ex is IOException || ex is SocketException)
                    throw NewProxyException(Resources.ProxyException_Error, ex);

                throw;
            }

            return curTcpClient;
        }


        #region Методы (внутренние защищённые)

        // ReSharper disable once SuggestBaseTypeForParameter
        private void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            var dstPort = GetIpAddressBytes(destinationHost);
            var dstIp = GetPortBytes(destinationPort);

            var userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            // +----+----+----+----+----+----+----+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+
            //    1    1      2              4           variable       1
            var request = new byte[9 + userId.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstIp.CopyTo(request, 2);
            dstPort.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //   1    1       2              4
            var response = new byte[8];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
                HandleCommandError(reply);
        }

        private byte[] GetIpAddressBytes(string destinationHost)
        {
            if (IPAddress.TryParse(destinationHost, out var ipAddress))
                return ipAddress.GetAddressBytes();

            try
            {
                var ips = Dns.GetHostAddresses(destinationHost);

                if (ips.Length > 0)
                    ipAddress = ips[0];
            }
            catch (Exception ex)
            {
                if (ex is SocketException || ex is ArgumentException)
                {
                    throw new ProxyException(string.Format(
                        Resources.ProxyException_FailedGetHostAddresses, destinationHost), this, ex);
                }

                throw;
            }

            return ipAddress.GetAddressBytes();
        }

        protected static byte[] GetPortBytes(int port)
        {
            var array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        protected void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case CommandReplyRequestRejectedOrFailed:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedOrFailed;
                    break;

                case CommandReplyRequestRejectedCannotConnectToIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedCannotConnectToIdentd;
                    break;

                case CommandReplyRequestRejectedDifferentIdentd:
                    errorMessage = Resources.Socks4_CommandReplyRequestRejectedDifferentIdentd;
                    break;

                default:
                    errorMessage = Resources.Socks_UnknownError;
                    break;
            }

            string exceptionMsg = string.Format(
                Resources.ProxyException_CommandError, errorMessage, ToString());

            throw new ProxyException(exceptionMsg, this);
        }

        #endregion
    }
}
