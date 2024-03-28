using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaf.xNet
{
    /// <summary>
    /// Статический класс с удобными методами для получения готовых реализаций TLS
    /// </summary>
    public static class TlsSettings
    {
        /// <summary>
        /// Возращает дефолтную .NET реализацию SSL/TLS
        /// <br/> <see cref="SystemTlsSettings"/>
        /// </summary>
        public static ITlsSettings SystemTls => new SystemTlsSettings();
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        public static ITlsSettings BouncyCastleDefaultTls => new BouncyCastleTlsSettings();
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        public static ITlsSettings BouncyCastleTls(ProtocolVersion[] versions) => new BouncyCastleTlsSettings() { SupportedVersions = versions };
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        public static ITlsSettings BouncyCastletTls(ProtocolVersion[] versions, List<int> supportedCiphers) => new BouncyCastleTlsSettings() { SupportedVersions = versions, SupportedCiphers = supportedCiphers };
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        public static ITlsSettings BouncyCastleTls(ProtocolVersion[] versions, List<int> supportedCiphers, List<int> supportedGroups) => new BouncyCastleTlsSettings() { SupportedVersions = versions, SupportedCiphers = supportedCiphers, SupportedGroups = supportedGroups };
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        public static ITlsSettings BouncyCastleTls(ProtocolVersion[] versions, List<int> supportedCiphers, List<int> supportedGroups, List<int> signatureSchemes) => new BouncyCastleTlsSettings() { SupportedVersions = versions, SupportedCiphers = supportedCiphers, SupportedGroups = supportedGroups, SignatureSchemes = signatureSchemes };
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle, эмулирует отпечаток JA3
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        /// <param name="ja3">Строка типа JA3, состоит из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентов. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code></param>
        public static ITlsSettings Ja3Tls(string ja3) => new Ja3TlsSettings(ja3);
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle, эмулирует отпечаток JA3
        /// <br/> <see cref="BouncyCastleTlsSettings"/>
        /// </summary>
        ///<param name="ja3">Строка типа JA3, состоит из пяти групп, разделенных запятой, которые состоят из десятичных цифр, разделенных знаком -, обозначающие параметры Client Hello, отправляемого клиентов. Порядок групп в JA3: <code>TLSVersion,Ciphers,Extensions,EllipticCurves,EllipticCurvePointFormats</code> Пример строки: <code>771,49196-49162-49195-52393-49161-49200-49172-49199-52392-49171-159-57-56-107-158-52394-51-50-103-22-19-157-53-61-156-47-60-10,0-23-65281-10-11-13-28,29-23-24-25,0</code></param>
        public static ITlsSettings Ja3Tls(string ja3, ProtocolVersion[] versions) => new Ja3TlsSettings(ja3) { SupportedVersions = versions };
        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle, эмулирует отпечаток JA3 от Firefox на 28.03.2024<br/><br/>Внимание: может устареть. Получить новый можно <a href="https://tls.browserleaks.com/tls">здесь</a>  
        /// </summary>
        public static ITlsSettings FirefoxTls => Ja3Tls("771,4865-4867-4866-49195-49199-52393-52392-49196-49200-49162-49161-49171-49172-156-157-47-53,0-23-65281-10-11-35-16-5-34-51-43-13-45-28-65037,29-23-24-25-256-257,0");

        /// <summary>
        /// Возращает реализацию SSL/TLS c помощью BouncyCastle, эмулирует отпечаток JA3 от Chrome на 28.03.2024<br/><br/>Внимание: может устареть. Получить новый можно <a href="https://tls.browserleaks.com/tls">здесь</a>
        /// </summary>
        public static ITlsSettings ChromeTls => Ja3Tls("771,4865-4866-4867-49195-49199-49196-49200-52393-52392-49171-49172-156-157-47-53,13-18-0-17513-65037-35-23-43-16-11-27-45-5-10-65281-51-21,29-23-24,0");
    }
}
