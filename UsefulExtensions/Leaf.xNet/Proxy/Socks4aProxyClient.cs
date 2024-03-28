using System.Net.Sockets;
using System.Text;

namespace Leaf.xNet
{
    /// <inheritdoc />
    /// <summary>
    /// Представляет клиент для Socks4a прокси-сервера.
    /// </summary>
    public sealed class Socks4AProxyClient : Socks4ProxyClient 
    {
        #region Конструкторы (открытые)

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4AProxyClient" />.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public Socks4AProxyClient()
            : this(null) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4AProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        public Socks4AProxyClient(string host, int port = DefaultPort)
            : this(host, port, string.Empty) { }

        /// <inheritdoc />
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:Leaf.xNet.Socks4AProxyClient" /> заданными данными о прокси-сервере.
        /// </summary>
        /// <param name="host">Хост прокси-сервера.</param>
        /// <param name="port">Порт прокси-сервера.</param>
        /// <param name="username">Имя пользователя для авторизации на прокси-сервере.</param>
        public Socks4AProxyClient(string host, int port, string username)
            : base(host, port, username)
        {
            _type = ProxyType.Socks4A;
        }

        #endregion


        #region Методы (открытые)

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks4AProxyClient"/>.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <returns>Экземпляр класса <see cref="Socks4AProxyClient"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="proxyAddress"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="proxyAddress"/> является пустой строкой.</exception>
        /// <exception cref="System.FormatException">Формат порта является неправильным.</exception>
        // ReSharper disable once UnusedMember.Global
        public new static Socks4AProxyClient Parse(string proxyAddress)
        {
            return Parse(ProxyType.Socks4A, proxyAddress) as Socks4AProxyClient;
        }

        /// <summary>
        /// Преобразует строку в экземпляр класса <see cref="Socks4AProxyClient"/>. Возвращает значение, указывающее, успешно ли выполнено преобразование.
        /// </summary>
        /// <param name="proxyAddress">Строка вида - хост:порт:имя_пользователя:пароль. Три последних параметра являются необязательными.</param>
        /// <param name="result">Если преобразование выполнено успешно, то содержит экземпляр класса <see cref="Socks4AProxyClient"/>, иначе <see langword="null"/>.</param>
        /// <returns>Значение <see langword="true"/>, если параметр <paramref name="proxyAddress"/> преобразован успешно, иначе <see langword="false"/>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse(string proxyAddress, out Socks4AProxyClient result)
        {
            if (!TryParse(ProxyType.Socks4A, proxyAddress, out var proxy))
            {
                result = null;
                return false;                
            }

            result = proxy as Socks4AProxyClient;
            return true;
        }

        #endregion


        // ReSharper disable once UnusedMember.Global
        internal void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
        {
            var dstPort = GetPortBytes(destinationPort);
            byte[] dstIp = { 0, 0, 0, 1 };

            var userId = string.IsNullOrEmpty(_username) ?
                new byte[0] : Encoding.ASCII.GetBytes(_username);

            var dstAddress = Encoding.ASCII.GetBytes(destinationHost);

            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            // | VN | CD | DSTPORT |      DSTIP        | USERID       |NULL| DSTADDR      |NULL|
            // +----+----+----+----+----+----+----+----+----+----+....+----+----+----+....+----+
            //    1    1      2              4           variable       1    variable        1 
            var request = new byte[10 + userId.Length + dstAddress.Length];

            request[0] = VersionNumber;
            request[1] = command;
            dstPort.CopyTo(request, 2);
            dstIp.CopyTo(request, 4);
            userId.CopyTo(request, 8);
            request[8 + userId.Length] = 0x00;
            dstAddress.CopyTo(request, 9 + userId.Length);
            request[9 + userId.Length + dstAddress.Length] = 0x00;

            nStream.Write(request, 0, request.Length);

            // +----+----+----+----+----+----+----+----+
            // | VN | CD | DSTPORT |      DSTIP        |
            // +----+----+----+----+----+----+----+----+
            //    1    1      2              4
            var response = new byte[8];

            nStream.Read(response, 0, 8);

            byte reply = response[1];

            // Если запрос не выполнен.
            if (reply != CommandReplyRequestGranted)
                HandleCommandError(reply);
        }
    }
}
