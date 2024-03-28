using Org.BouncyCastle.Tls;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Leaf.xNet
{
    /// <summary>
    /// Дефолтные настройки TLS для <see cref="HttpRequest"/>, используется <see cref="SslStream"/>
    /// </summary>
    public class SystemTlsSettings : ITlsSettings
    {
        /// <summary>
        /// Возвращает или задает метод делегата, вызываемый при проверки сертификата SSL, используемый для проверки подлинности.
        /// </summary>
        /// <value>Значение по умолчанию — <see langword="null"/>. Если установлено значение по умолчанию, то используется метод, который принимает все сертификаты SSL.</value>
        public RemoteCertificateValidationCallback SslCertificateValidatorCallback { get; set; }

        /// <summary>
        /// Возвращает или задает возможные протоколы SSL.
        /// По умолчанию используется: <value>SslProtocols.Tls | SslProtocols.Tls12 | SslProtocols.Tls11</value>.
        /// </summary>
        public SslProtocols SslProtocols { get; set; } 

        /// <inheritdoc/>
        public Stream GetTlsStream(string addressHost, Stream networkStream)
        {
            var sslStream = SslCertificateValidatorCallback == null
                            ? new SslStream(networkStream, false, Http.AcceptAllCertificationsCallback)
                            : new SslStream(networkStream, false, SslCertificateValidatorCallback);

            sslStream.AuthenticateAsClient(addressHost, new X509CertificateCollection(), SslProtocols, false);

            return sslStream;
        }
        /// <inheritdoc/>

        public void Dispose()
        {
        }
    }
}
