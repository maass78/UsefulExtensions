using Org.BouncyCastle.Tls;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Leaf.xNet
{

    /// <summary>
    /// Обычные настройки TLS с помощью библиотеки BouncyCastle и её реализации TLS
    /// </summary>
    public class BouncyCastleTlsSettings : ITlsSettings
    {
        /// <summary>
        /// Констуктор
        /// </summary>
        public BouncyCastleTlsSettings()
        {
            SignatureSchemes = new List<int>()
            {
                SignatureScheme.ecdsa_secp256r1_sha256,
                SignatureScheme.rsa_pss_rsae_sha256,
                SignatureScheme.rsa_pkcs1_sha256,
                SignatureScheme.ecdsa_secp384r1_sha384,
                SignatureScheme.rsa_pss_rsae_sha384,
                SignatureScheme.rsa_pkcs1_sha384,
                SignatureScheme.rsa_pss_rsae_sha512,
                SignatureScheme.rsa_pkcs1_sha512,
                SignatureScheme.rsa_pkcs1_sha1,
            };

            SupportedCiphers = new List<int>()
            {
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_CHACHA20_POLY1305_SHA256,
                CipherSuite.TLS_AES_128_GCM_SHA256,
                CipherSuite.TLS_AES_256_GCM_SHA384,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384,
                CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
                CipherSuite.TLS_EMPTY_RENEGOTIATION_INFO_SCSV
            };

            SupportedGroups = new List<int>()
            {
                NamedGroup.x25519,
                NamedGroup.secp256r1,
                NamedGroup.secp384r1,
            };

            SupportedVersions = ProtocolVersion.TLSv13.DownTo(ProtocolVersion.TLSv10);
        }

        /// <summary>
        /// SignatureSchemes, в душе не ебу че это, но копировать надо
        /// </summary>
        public List<int> SignatureSchemes { get; set; }

        /// <summary>
        /// Список шифронаборов
        /// </summary>
        public List<int> SupportedCiphers { get; set; }

        /// <summary>
        /// Список Supported Groups, они же - elliptic_curves
        /// </summary>
        public List<int> SupportedGroups { get; set; }

        /// <summary>
        /// Поддерживаемые версии TLS
        /// </summary>
        public ProtocolVersion[] SupportedVersions { get; set; }

        protected TlsClientProtocol protocol;

        /// <summary>
        /// Создать tls клиент с данными настройками
        /// </summary>
        /// <returns></returns>
        public virtual AbstractTlsClient GetTlsClient(string[] serverNames)
        {
            return new BouncyCastleTlsClient()
            {
                SignatureAlgorithms = SignatureSchemes.Select(x => CreateSignatureAlgorithm(x)).ToArray(),
                SupportedCiphers = SupportedCiphers.ToArray(),
                SupportedGroups = SupportedGroups.ToArray(),
                SupportedVersions = SupportedVersions,
                ServerNames = serverNames
            };
        }

        /// <summary>
        /// Создать TLS протокол для использования 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public virtual TlsClientProtocol GetTlsProtocol(Stream stream)
        {
            return new TlsClientProtocol(stream);
        }

        protected static SignatureAndHashAlgorithm CreateSignatureAlgorithm(int signatureScheme)
        {
            short hashAlgorithm = SignatureScheme.GetHashAlgorithm(signatureScheme);
            short signatureAlgorithm = SignatureScheme.GetSignatureAlgorithm(signatureScheme);
            return new SignatureAndHashAlgorithm(hashAlgorithm, signatureAlgorithm);
        }
        /// <inheritdoc/>
        public virtual Stream GetTlsStream(string addressHost, Stream networkStream)
        {
            var protocol = GetTlsProtocol(networkStream);

            protocol.Connect(GetTlsClient(new string[] { addressHost }));

            this.protocol = protocol;

            return protocol.Stream;
        }

        public void Dispose()
        {
            if (protocol != null)
            {
                protocol.Stream?.Dispose();
                protocol.Close();
                protocol = null;
            }
        }
    }
}
