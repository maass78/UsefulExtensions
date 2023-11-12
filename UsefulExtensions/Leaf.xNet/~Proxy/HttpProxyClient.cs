using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет клиент для HTTP прокси-сервера.
    /// </summary>
    public sealed class HttpProxyClient : ProxyClient
    {
        #region Константы (закрытые)

        private const int BufferSize = 50;
        private const int DefaultPort = 8080;

        #endregion

        // TODO: hide constructors and make ProxyClient Factory: ProxyClient.ParseHttp / ProxyClient.ParseSocks4 / ProxyClient.ParseSocks5

        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpProxyClient" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public HttpProxyClient()
            : this(null) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        public HttpProxyClient(string host, int port = DefaultPort)
            : this(host, port, string.Empty, string.Empty) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.HttpProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        /// <param name="username">Имя пользователя для авторизации на прокси-сервере.</param>
        /// <param name="password">Пароль для авторизации на прокси-сервере.</param>
        public HttpProxyClient(string host, int port, string username, string password)
            : base(ProxyType.HTTP, host, port, username, password) { }

        #endregion


        #region Статические свойства (открытые)
        /// <summary>
        /// Версия протокола которая должна использоваться. HTTP 2.0 не поддерживается в данный момент.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public static string ProtocolVersion { get; set; } = "1.1"; 
        
        #endregion


        #region Статические методы (открытые)

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="HttpProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <returns>Экземпляр класса <see cref="HttpProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="proxyAddress"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="proxyAddress"/> является пустой строкой.</exception>
        /// <exception cref="System.FormatException">Формат порта является неправильным.</exception>
        // ReSharper disable once UnusedMember.Global
        public new static HttpProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.HTTP, proxyAddress) as HttpProxyClient;
        }

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="HttpProxyClient"/>. Возвращает значение, указывающее, успешно ли выполнено преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <param name="result">Если преобразование выполнено успешно, то содержит экземпляр класса <see cref="HttpProxyClient"/>, иначе <see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешно, иначе <see langword="false"/>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse(string proxyAddress, out HttpProxyClient result)
        {
            if (!TryParse(ProxyType.HTTP, proxyAddress, out var proxy))
            {
                result = null;
                return false;
            }

            result = proxy as HttpProxyClient;
            return true;
        }

        #endregion

        #region Методы (открытые)

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
        /// <remarks>Если порт сервера неравен 80, то для подключения используется метод 'CONNECT'.</remarks>
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

            if (destinationPort == 80)
                return curTcpClient;

            HttpStatusCode statusCode;

            try
            {
                var nStream = curTcpClient.GetStream();

                SendConnectionCommand(nStream, destinationHost, destinationPort);
                statusCode = ReceiveResponse(nStream);
            }
            catch (Exception ex)
            {
                curTcpClient.Close();

                if (ex is IOException || ex is SocketException)
                    throw NewProxyException(Resources.ProxyException_Error, ex);

                throw;
            }

            if (statusCode == HttpStatusCode.OK)
                return curTcpClient;

            curTcpClient.Close();

            throw new ProxyException(string.Format(
                Resources.ProxyException_ReceivedWrongStatusCode, statusCode, ToString()), this);
        }

        #endregion


        #region Методы (закрытые)

        private string GenerateAuthorizationHeader()
        {
            if (string.IsNullOrEmpty(_username) && string.IsNullOrEmpty(_password))
                return string.Empty;

            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{_username}:{_password}"));

            return $"Proxy-Authorization: Basic {data}\r\n";
        }

        private void SendConnectionCommand(Stream nStream, string destinationHost, int destinationPort)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CONNECT {0}:{1} HTTP/{2}\r\n", destinationHost, destinationPort, ProtocolVersion);
            sb.AppendFormat(GenerateAuthorizationHeader());
            sb.Append("Host: "); sb.AppendLine(destinationHost);
            sb.AppendLine("Proxy-Connection: Keep-Alive");
            sb.AppendLine();

            var buffer = Encoding.ASCII.GetBytes(sb.ToString());
            nStream.Write(buffer, 0, buffer.Length);
        }

        private HttpStatusCode ReceiveResponse(NetworkStream nStream)
        {
            var buffer = new byte[BufferSize];
            var responseBuilder = new StringBuilder();

            WaitData(nStream);

            do
            {
                int bytesRead = nStream.Read(buffer, 0, BufferSize);
                responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            } while (nStream.DataAvailable);

            string response = responseBuilder.ToString();

            if (response.Length == 0)
                throw NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);

            // Выделяем строку статуса. Пример: HTTP/1.1 200 OK\r\n
            string strStatus = response.Substring(" ", Http.NewLine);
            if (strStatus == null)
                throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);

            int simPos = strStatus.IndexOf(' ');
            if (simPos == -1)
                throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);

            string statusLine = strStatus.Substring(0, simPos);

            if (statusLine.Length == 0)
                throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);

            return Enum.TryParse(statusLine, out HttpStatusCode statusCode) 
                ? statusCode
                : HttpStatusCode.InvalidStatusCode;
        }

        private void WaitData(NetworkStream nStream)
        {
            int sleepTime = 0;
            int delay = nStream.ReadTimeout < 10 ?
                10 : nStream.ReadTimeout;

            while (!nStream.DataAvailable)
            {
                if (sleepTime >= delay)
                    throw NewProxyException(Resources.ProxyException_WaitDataTimeout);

                sleepTime += 10;
                Thread.Sleep(10);
            }
        }

        #endregion
    }
}
