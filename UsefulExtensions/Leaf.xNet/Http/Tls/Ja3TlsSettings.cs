using Org.BouncyCastle.Tls;
using System.IO;
using System.Linq;

namespace Leaf.xNet
{
    /// <summary>
    /// Настройки TLS, которые можно установить с помощью строки типа JA3. Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a><br/><br/>
    /// Почитать подробнее можно: <br/> <a href="https://habr.com/ru/companies/acribia/articles/560168/">ТЫК</a>  <br/> <a href="https://habr.com/ru/articles/596411/">ТЫК</a> <br/><br/> Используется <a href="https://google.com">форк BouncyCastle</a> 
    /// </summary>
    public class Ja3TlsSettings : BouncyCastleTlsSettings
    {
        /// <summary>
        /// Строка типа JA3. Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a>
        /// </summary>
        public string Ja3 { get; protected set; }

        /// <summary>
        /// Версия TLS record, смотреть какая нужна в Wireshark
        /// </summary>
        public ProtocolVersion RecordVersion { get; protected set; } = ProtocolVersion.TLSv10;
        /// <summary>
        /// Конструктор <see cref="Ja3TlsSettings"/>, принимает строку типа JA3, состоящую из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентом. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code><br/><br/>Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a>
        /// </summary>
        /// <param name="ja3">Строка типа JA3, состоит из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентов. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code><br/><br/>Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a></param>
        public Ja3TlsSettings(string ja3)
        {
            Ja3 = ja3;
        }
        /// <summary>
        /// Конструктор <see cref="Ja3TlsSettings"/>, принимает строку типа JA3, состоящую из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентом. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code><br/><br/>Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a>
        /// </summary>
        /// <param name="ja3">Строка типа JA3, состоит из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентов. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code><br/><br/>Получить свой можно <a href="https://tls.browserleaks.com/tls">здесь</a></param>
        /// <param name="recordVersion">Версия Tls Record</param>
        public Ja3TlsSettings(string ja3, ProtocolVersion recordVersion)
        {
            Ja3 = ja3;
            RecordVersion = recordVersion;
        }
        /// <inheritdoc/>
        public override AbstractTlsClient GetTlsClient(string[] serverNames)
        {
            return new Ja3TlsClient(serverNames, Ja3);
        }
        /// <inheritdoc/>
        public override TlsClientProtocol GetTlsProtocol(Stream stream)
        {
            var parts = Ja3.Split(',');
            var extensions = parts[2].Split('-').Select(x => int.Parse(x)).ToArray();

            return new TlsJa3Protocol(stream, extensions);
        }
    }
}
