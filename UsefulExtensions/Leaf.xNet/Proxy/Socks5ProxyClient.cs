using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет клиент для Socks5 прокси-сервера.
    /// </summary>
    public sealed class Socks5ProxyClient : ProxyClient
    {
        #region Константы (закрытые)

        private const int DefaultPort = 1080;

        private const byte VersionNumber = 5;
        private const byte Reserved = 0x00;
        private const byte AuthMethodNoAuthenticationRequired = 0x00;
        //private const byte AuthMethodGssapi = 0x01;
        private const byte AuthMethodUsernamePassword = 0x02;
        //private const byte AuthMethodIanaAssignedRangeBegin = 0x03;
        //private const byte AuthMethodIanaAssignedRangeEnd = 0x7f;
        //private const byte AuthMethodReservedRangeBegin = 0x80;
        //private const byte AuthMethodReservedRangeEnd = 0xfe;
        private const byte AuthMethodReplyNoAcceptableMethods = 0xff;
        private const byte CommandConnect = 0x01;
        //private const byte CommandBind = 0x02;
        //private const byte CommandUdpAssociate = 0x03;
        private const byte CommandReplySucceeded = 0x00;
        private const byte CommandReplyGeneralSocksServerFailure = 0x01;
        private const byte CommandReplyConnectionNotAllowedByRuleset = 0x02;
        private const byte CommandReplyNetworkUnreachable = 0x03;
        private const byte CommandReplyHostUnreachable = 0x04;
        private const byte CommandReplyConnectionRefused = 0x05;
        // ReSharper disable once InconsistentNaming
        private const byte CommandReplyTTLExpired = 0x06;
        private const byte CommandReplyCommandNotSupported = 0x07;
        private const byte CommandReplyAddressTypeNotSupported = 0x08;
        private const byte AddressTypeIPv4 = 0x01;
        private const byte AddressTypeDomainName = 0x03;
        private const byte AddressTypeIPv6 = 0x04;

        #endregion


        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks5ProxyClient" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public Socks5ProxyClient()
            : this(null) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks5ProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        public Socks5ProxyClient(string host, int port = DefaultPort)
            : this(host, port, string.Empty, string.Empty) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks5ProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        /// <param name="username">Имя пользователя для авторизации на прокси-сервере.</param>
        /// <param name="password">Пароль для авторизации на прокси-сервере.</param>
        public Socks5ProxyClient(string host, int port, string username, string password)
            : base(ProxyType.Socks5, host, port, username, password) { }

        #endregion


        #region Статические методы (открытые)

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks5ProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <returns>Экземпляр класса <see cref="Socks5ProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="proxyAddress"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="proxyAddress"/> является пустой строкой.</exception>
        /// <exception cref="System.FormatException">Формат порта является неправильным.</exception>
        // ReSharper disable once UnusedMember.Global
        public new static Socks5ProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.Socks5, proxyAddress) as Socks5ProxyClient;
        }

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks5ProxyClient"/>. Возвращает значение, указывающее, успешно ли выполнено преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <param name="result">Если преобразование выполнено успешно, то содержит экземпляр класса <see cref="Socks5ProxyClient"/>, иначе <see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешно, иначе <see langword="false"/>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse(string proxyAddress, out Socks5ProxyClient result)
        {
            if (!ProxyClient.TryParse(ProxyType.Socks5, proxyAddress, out var proxy))
            {
                result = null;
                return false;
                
            }

            result = proxy as Socks5ProxyClient;
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
                var nStream = curTcpClient.GetStream();

                InitialNegotiation(nStream);
                SendCommand(nStream, CommandConnect, destinationHost, destinationPort);
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


        #region Методы (закрытые)

        private void InitialNegotiation(Stream nStream)
        {
            byte authMethod = !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)
                ? AuthMethodUsernamePassword
                : AuthMethodNoAuthenticationRequired;

            // +----+----------+----------+
            // |VER | NMETHODS | METHODS  |
            // +----+----------+----------+
            // | 1  |    1     | 1 to 255 |
            // +----+----------+----------+
            var request = new byte[3];

            request[0] = VersionNumber;
            request[1] = 1;
            request[2] = authMethod;

            nStream.Write(request, 0, request.Length);

            // +----+--------+
            // |VER | METHOD |
            // +----+--------+
            // | 1  |   1    |
            // +----+--------+
            var response = new byte[2];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];

            if (authMethod == AuthMethodUsernamePassword && reply == AuthMethodUsernamePassword)
                SendUsernameAndPassword(nStream);
            else if (reply != CommandReplySucceeded)
                HandleCommandError(reply);
        }

        private void SendUsernameAndPassword(Stream nStream)
        {
            var username = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            var password = string.IsNullOrEmpty(_password) ?
                new byte[0] : Encoding.ASCII.GetBytes(_password);

            // +----+------+----------+------+----------+
            // |VER | ULEN |  UNAME   | PLEN |  PASSWD  |
            // +----+------+----------+------+----------+
            // | 1  |  1   | 1 to 255 |  1   | 1 to 255 |
            // +----+------+----------+------+----------+
            var request = new byte[username.Length + password.Length + 3];

            request[0] = 1;
            request[1] = (byte)username.Length;
            username.CopyTo(request, 2);
            request[2 + username.Length] = (byte)password.Length;
            password.CopyTo(request, 3 + username.Length);

            nStream.Write(request, 0, request.Length);

            // +----+--------+
            // |VER | STATUS |
            // +----+--------+
            // | 1  |   1    |
            // +----+--------+
            var response = new byte[2];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];
            if (reply != CommandReplySucceeded)
                throw NewProxyException(Resources.ProxyException_Socks5_FailedAuthOn);
        }

        private void SendCommand(Stream nStream, byte command, string destinationHost, int destinationPort)
        {
            byte aTyp = GetAddressType(destinationHost);
            var dstAddress = GetAddressBytes(aTyp, destinationHost);
            var dstPort = GetPortBytes(destinationPort);

            // +----+-----+-------+------+----------+----------+
            // |VER | CMD |  RSV  | ATYP | DST.ADDR | DST.PORT |
            // +----+-----+-------+------+----------+----------+
            // | 1  |  1  | X'00' |  1   | Variable |    2     |
            // +----+-----+-------+------+----------+----------+
            var request = new byte[4 + dstAddress.Length + 2];

            request[0] = VersionNumber;
            request[1] = command;
            request[2] = Reserved;
            request[3] = aTyp;
            dstAddress.CopyTo(request, 4);
            dstPort.CopyTo(request, 4 + dstAddress.Length);

            nStream.Write(request, 0, request.Length);

            // +----+-----+-------+------+----------+----------+
            // |VER | REP |  RSV  | ATYP | BND.ADDR | BND.PORT |
            // +----+-----+-------+------+----------+----------+
            // | 1  |  1  | X'00' |  1   | Variable |    2     |
            // +----+-----+-------+------+----------+----------+
            var response = new byte[255];

            nStream.Read(response, 0, response.Length);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplySucceeded)
                HandleCommandError(reply);
        }

        private byte GetAddressType(string host)
        {
            if (!IPAddress.TryParse(host, out var ipAddress))
                return AddressTypeDomainName;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ipAddress.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return AddressTypeIPv4;

                case AddressFamily.InterNetworkV6:
                    return AddressTypeIPv6;

                default:
                    throw new ProxyException(string.Format(Resources.ProxyException_NotSupportedAddressType,
                        host, Enum.GetName(typeof(AddressFamily), ipAddress.AddressFamily), ToString()), this);
            }
        }

        private static byte[] GetAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case AddressTypeIPv4:
                case AddressTypeIPv6:
                    return IPAddress.Parse(host).GetAddressBytes();

                case AddressTypeDomainName:
                    var bytes = new byte[host.Length + 1];

                    bytes[0] = (byte)host.Length;
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);

                    return bytes;

                default:
                    return null;
            }
        }

        private static byte[] GetPortBytes(int port)
        {
            var array = new byte[2];

            array[0] = (byte)(port / 256);
            array[1] = (byte)(port % 256);

            return array;
        }

        private void HandleCommandError(byte command)
        {
            string errorMessage;

            switch (command)
            {
                case AuthMethodReplyNoAcceptableMethods:
                    errorMessage = Resources.Socks5_AuthMethodReplyNoAcceptableMethods;
                    break;

                case CommandReplyGeneralSocksServerFailure:
                    errorMessage = Resources.Socks5_CommandReplyGeneralSocksServerFailure;
                    break;

                case CommandReplyConnectionNotAllowedByRuleset:
                    errorMessage = Resources.Socks5_CommandReplyConnectionNotAllowedByRuleset;
                    break;

                case CommandReplyNetworkUnreachable:
                    errorMessage = Resources.Socks5_CommandReplyNetworkUnreachable;
                    break;

                case CommandReplyHostUnreachable:
                    errorMessage = Resources.Socks5_CommandReplyHostUnreachable;
                    break;

                case CommandReplyConnectionRefused:
                    errorMessage = Resources.Socks5_CommandReplyConnectionRefused;
                    break;

                case CommandReplyTTLExpired:
                    errorMessage = Resources.Socks5_CommandReplyTTLExpired;
                    break;

                case CommandReplyCommandNotSupported:
                    errorMessage = Resources.Socks5_CommandReplyCommandNotSupported;
                    break;

                case CommandReplyAddressTypeNotSupported:
                    errorMessage = Resources.Socks5_CommandReplyAddressTypeNotSupported;
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
