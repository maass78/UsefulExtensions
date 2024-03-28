using Org.BouncyCastle.Tls;
using System;
using System.IO;
using System.Net.Security;

namespace Leaf.xNet
{
    /// <summary>
    /// Интерфейс для настроек TLS
    /// </summary>
    public interface ITlsSettings : IDisposable
    {
        /// <summary>
        /// Возвращает поток, используемый для общения с сервером по протоколу TLS
        /// </summary>
        /// <param name="addressHost">Доменное имя сервера</param>
        /// <param name="networkStream">Сетевой поток</param>
        /// <returns></returns>
        Stream GetTlsStream(string addressHost, Stream networkStream);
    }
}
