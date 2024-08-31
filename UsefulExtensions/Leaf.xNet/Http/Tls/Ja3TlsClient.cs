using Leaf.xNet;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using static Leaf.xNet.BouncyCastleTlsClient;

namespace Leaf.xNet
{
    /// <summary>
    /// TLS клиент, наследуемый от <see cref="DefaultTlsClient"/>, принимающий строку типа JA3, с помощью которого устанавливаются параметры TLS
    /// Для правильной работы необходима измененная версия BouncyCastle
    /// </summary>
    public class Ja3TlsClient : DefaultTlsClient
    {
        /// <summary>
        /// Доменное имя
        /// </summary>
        public string[] ServerNames
        {
            set
            {
                if (value == null)
                {
                    _serverNames = null;
                }
                else
                {
                    _serverNames = value.Select(x => new ServerName(NameType.host_name, Encoding.ASCII.GetBytes(x))).ToArray();
                }
            }
        }
        /// <summary>
        /// Список используемых расширений TLS, обозначается их порядок, их значения устанавливаются либо с помощью свойств, таких как <see cref="SignatureAlgorithms"/>, <see cref="SupportedGroups"/>, либо установлены по умолчанию в методе <see cref="GetClientExtensions"/>
        /// </summary>
        public int[] ExtensionsOrder { get; set; }
        /// <summary>
        /// Список SignatureAlgorithms
        /// </summary>
        public IList<SignatureAndHashAlgorithm> SignatureAlgorithms { get; set; } = new[] {
             CreateSignatureAlgorithm(SignatureScheme.ecdsa_secp256r1_sha256),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pss_rsae_sha256),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pkcs1_sha256),
             CreateSignatureAlgorithm(SignatureScheme.ecdsa_secp384r1_sha384),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pss_rsae_sha384),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pkcs1_sha384),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pss_rsae_sha512),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pkcs1_sha512),
             CreateSignatureAlgorithm(SignatureScheme.rsa_pkcs1_sha1),
         };
        /// <summary>
        /// Список шифронаборов
        /// </summary>
        public int[] SupportedCiphers { get; set; } = new[] {
             CipherSuite.TLS_CHACHA20_POLY1305_SHA256,
             CipherSuite.TLS_AES_128_GCM_SHA256,
             CipherSuite.TLS_AES_256_GCM_SHA384,
             CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256,
             CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
             CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
             CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
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
        /// <summary>
        /// Список Supported Groups, они же - elliptic_curves
        /// </summary>
        public int[] SupportedGroups { get; set; } = new[] {
             NamedGroup.x25519,
             NamedGroup.secp256r1,
             NamedGroup.secp384r1,
         };
        /// <summary>
        /// Поддерживаемые версии защищенного соединения SSL/TLS
        /// </summary>
        public ProtocolVersion[] SupportedVersions { get; set; } = ProtocolVersion.TLSv13.DownTo(ProtocolVersion.TLSv12);
        /// <summary>
        /// Строка JA3, отпечаток TLS 
        /// </summary>
        public string JA3 { get; private set; }

        private ServerName[] _serverNames;

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="serverNames">доменные имена для текущего соединения</param>
        /// <param name="ja3">строка формата JA3, отпечаток TLS</param>
        /// <exception cref="FormatException">Вызывается, если <paramref name="ja3"/> указан неверно</exception>
        public Ja3TlsClient(string[] serverNames, string ja3) : base(new BcTlsCrypto(new SecureRandom()))
        {
            ServerNames = serverNames;
            JA3 = ja3;

            string[] ja3parts = JA3.Split(',');

            if (ja3parts.Length != 5)
                throw new FormatException("JA3 isn't in correct format");

            SetWithJA3(ja3parts);
        }

        private void SetWithJA3(string[] ja3)
        {
            var tlsVersion = int.Parse(ja3[0]);
            var ciphers = ja3[1].Split('-').Select(x => int.Parse(x)).ToArray();
            var extensions = ja3[2].Split('-').Select(x => int.Parse(x)).ToArray();
            var ellipticCurve = ja3[3].Split('-').Select(x => int.Parse(x)).ToArray();

            SupportedCiphers = ciphers;
            SupportedGroups = ellipticCurve;
            ExtensionsOrder = extensions;
        }
        public override IDictionary<int, byte[]> GetClientExtensions()
        {
            var clientExtensions = new Dictionary<int, byte[]>();

            bool offeringTlsV13Plus = false;
            bool offeringPreTlsV13 = false;
            {
                ProtocolVersion[] supportedVersions = GetProtocolVersions();
                for (int i = 0; i < supportedVersions.Length; ++i)
                {
                    if (TlsUtilities.IsTlsV13(supportedVersions[i]))
                    {
                        offeringTlsV13Plus = true;
                    }
                    else
                    {
                        offeringPreTlsV13 = true;
                    }
                }
            }

            var pskKeyExchangeModes = GetPskKeyExchangeModes();
            if(pskKeyExchangeModes != null)
            {
                TlsExtensionsUtilities.AddPskKeyExchangeModesExtension(clientExtensions, pskKeyExchangeModes);
            }

            var protocolNames = GetProtocolNames();
            if (protocolNames != null)
            {
                TlsExtensionsUtilities.AddAlpnExtensionClient(clientExtensions, protocolNames);
            }

            var sniServerNames = GetSniServerNames();
            if (sniServerNames != null)
            {
                TlsExtensionsUtilities.AddServerNameExtensionClient(clientExtensions, sniServerNames);
            }

            CertificateStatusRequest statusRequest = GetCertificateStatusRequest();
            if (statusRequest != null)
            {
                TlsExtensionsUtilities.AddStatusRequestExtension(clientExtensions, statusRequest);
            }

            if (offeringTlsV13Plus)
            {
                var certificateAuthorities = GetCertificateAuthorities();
                if (certificateAuthorities != null)
                {
                    TlsExtensionsUtilities.AddCertificateAuthoritiesExtension(clientExtensions, certificateAuthorities);
                }
            }

            if (offeringPreTlsV13)
            {
                // TODO Shouldn't add if no offered cipher suite uses a block cipher?
                TlsExtensionsUtilities.AddEncryptThenMacExtension(clientExtensions);

                var statusRequestV2 = GetMultiCertStatusRequest();
                if (statusRequestV2 != null)
                {
                    TlsExtensionsUtilities.AddStatusRequestV2Extension(clientExtensions, statusRequestV2);
                }

                var trustedCAKeys = GetTrustedCAIndication();
                if (trustedCAKeys != null)
                {
                    TlsExtensionsUtilities.AddTrustedCAKeysExtensionClient(clientExtensions, trustedCAKeys);
                }
            }

            ProtocolVersion clientVersion = m_context.ClientVersion;

            /*
             * RFC 5246 7.4.1.4.1. Note: this extension is not meaningful for TLS versions prior to 1.2.
             * Clients MUST NOT offer it if they are offering prior versions.
             */
            if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(clientVersion))
            {
                var supportedSigAlgs = GetSupportedSignatureAlgorithms();
                if (null != supportedSigAlgs && supportedSigAlgs.Count > 0)
                {
                    this.m_supportedSignatureAlgorithms = supportedSigAlgs;

                    TlsExtensionsUtilities.AddSignatureAlgorithmsExtension(clientExtensions, supportedSigAlgs);
                }

                var supportedSigAlgsCert = GetSupportedSignatureAlgorithmsCert();
                if (null != supportedSigAlgsCert && supportedSigAlgsCert.Count > 0)
                {
                    this.m_supportedSignatureAlgorithmsCert = supportedSigAlgsCert;

                    TlsExtensionsUtilities.AddSignatureAlgorithmsCertExtension(clientExtensions, supportedSigAlgsCert);
                }
            }

            var namedGroupRoles = GetNamedGroupRoles();

            var supportedGroups = GetSupportedGroups(namedGroupRoles);
            if (supportedGroups != null && supportedGroups.Count > 0)
            {
                this.m_supportedGroups = supportedGroups;

                TlsExtensionsUtilities.AddSupportedGroupsExtension(clientExtensions, supportedGroups);
            }

            if (offeringPreTlsV13)
            {
                if (namedGroupRoles.Contains(NamedGroupRole.ecdh) ||
                    namedGroupRoles.Contains(NamedGroupRole.ecdsa))
                {
                    TlsExtensionsUtilities.AddSupportedPointFormatsExtension(clientExtensions,
                        new short[] { ECPointFormat.uncompressed });
                }
            }

            TlsExtensionsUtilities.AddMaxFragmentLengthExtension(clientExtensions, MaxFragmentLength.pow2_9);
            TlsExtensionsUtilities.AddPaddingExtension(clientExtensions, m_context.Crypto.SecureRandom.Next(16));
            TlsExtensionsUtilities.AddTruncatedHmacExtension(clientExtensions);
            TlsExtensionsUtilities.AddRecordSizeLimitExtension(clientExtensions, 16385);
            TlsExtensionsUtilities.AddAlpnExtensionClient(clientExtensions, new List<ProtocolName>() { ProtocolName.Http_1_1 });

            clientExtensions[27] = TlsUtilities.EncodeUint16ArrayWithUint8Length(new int[] { 0x3a3a });
            clientExtensions[34] = new byte[] { 00, 0x08, 0x04, 0x03, 0x05, 0x03, 0x06, 0x03, 0x02, 0x03 };
            clientExtensions[ExtensionType.renegotiation_info] = TlsUtilities.EncodeOpaque8(TlsUtilities.EmptyBytes);
            clientExtensions[17513] = new byte[0];
            clientExtensions[65037] = new byte[] { 00, 0x00, 0x01, 0x00, 0x01, 0xee, 0x00, 0x20, 0x96, 0x70, 0xa0, 0x08, 0x35, 0x06, 0xe7, 0x1a, 0xf9, 0xc3, 0x74, 0x79, 0xc5, 0xdd, 0x25, 0x5a, 0xc5, 0x60, 0xb7, 0x36, 0x0e, 0xd3, 0xe9, 0xf8, 0xb9, 0xf9, 0x04, 0xc5, 0xe3, 0x1e, 0x5c, 0xfd, 0x00, 0xef, 0x40, 0xd1, 0x4e, 0x1b, 0x9a, 0x5e, 0x0c, 0x1e, 0x88, 0x09, 0xcc, 0x7b, 0x50, 0x67, 0x81, 0xb5, 0x44, 0xf7, 0x5a, 0xfe, 0xa5, 0x3c, 0xba, 0x49, 0xc7, 0x7e, 0xd7, 0x36, 0x6b, 0xd3, 0xba, 0xd3, 0x84, 0xc9, 0x05, 0x56, 0x32, 0x0e, 0x02, 0xda, 0xd2, 0x84, 0x94, 0x39, 0x2f, 0x39, 0x8e, 0x76, 0xce, 0x7b, 0xc0, 0x11, 0x14, 0xd2, 0xa0, 0x3e, 0xa2, 0x34, 0x7f, 0x44, 0xe1, 0x4c, 0xb2, 0x90, 0xe4, 0xfb, 0xe2, 0xdf, 0x5b, 0x14, 0xe4, 0x8e, 0x4e, 0x08, 0xb6, 0x3d, 0x24, 0x1f, 0x85, 0xe5, 0x98, 0xd2, 0x05, 0x9a, 0x30, 0x3b, 0xdf, 0x1c, 0xa9, 0x19, 0x9a, 0x65, 0x9d, 0xc7, 0xbd, 0x67, 0x6d, 0xd2, 0x74, 0x39, 0xbd, 0xed, 0x8b, 0xcf, 0xb8, 0xce, 0xdd, 0x0c, 0x7c, 0x45, 0xbe, 0xa8, 0x7e, 0xf6, 0x10, 0xa5, 0xaa, 0x8a, 0xc0, 0x45, 0xa2, 0xf1, 0xa3, 0xa8, 0xc4, 0x15, 0xff, 0x0d, 0x38, 0xb7, 0x46, 0xfd, 0xab, 0xfe, 0xed, 0xe6, 0x9d, 0xad, 0x8b, 0xee, 0xa3, 0x00, 0xe6, 0x6a, 0xb1, 0x97, 0xc9, 0x69, 0x48, 0x0a, 0x38, 0x6e, 0x71, 0x6e, 0x8c, 0xe2, 0xcc, 0x99, 0x05, 0x87, 0x44, 0xe0, 0x2b, 0x0d, 0x84, 0x23, 0x1d, 0x38, 0x13, 0x42, 0x85, 0x3d, 0x84, 0x32, 0x23, 0x15, 0x91, 0x00, 0xaf, 0x5b, 0x7f, 0x56, 0x74, 0x87, 0xe0, 0xb4, 0x8e, 0x94, 0xe1, 0xe0, 0x49, 0x7a, 0x3b, 0xd6, 0x15, 0x8c, 0x2e, 0x9b, 0xe9, 0xb7, 0x65, 0xdc, 0xe8, 0x43, 0xbe, 0x48, 0x7e, 0xb3, 0x3e, 0x32, 0x6e, 0xa0, 0x5f, 0xaa, 0x2e, 0x8d, 0xba, 0x67, 0xaa, 0xbf, 0xf8, 0x57, 0xba, 0xc1, 0x73, 0x88, 0x38, 0xcc, 0x29, 0x22, 0x6c, 0x1e, 0xdc, 0x5d, 0xae, 0x7f, 0x2f, 0x42, 0xc5 };

            for (int i = 0; i < ExtensionsOrder.Length; i++)
            {
                if (!clientExtensions.ContainsKey(ExtensionsOrder[i]))
                {
                    if (ExtensionsOrder[i] != 27)
                        clientExtensions.Add(ExtensionsOrder[i], new byte[0]);
                }
            }

            return clientExtensions;
        }
        public override TlsAuthentication GetAuthentication() => new EmptyTlsAuthentication();
        protected override ProtocolVersion[] GetSupportedVersions() => SupportedVersions;
        protected override IList<SignatureAndHashAlgorithm> GetSupportedSignatureAlgorithms() => SignatureAlgorithms;
        protected override int[] GetSupportedCipherSuites() => SupportedCiphers;
        protected override IList<ServerName> GetSniServerNames() => _serverNames;
        protected override IList<int> GetSupportedGroups(IList<int> namedGroupRoles)
        {
            //return SupportedGroups;
            var supportedGroups = new List<int>();
            TlsUtilities.AddIfSupported(supportedGroups, Crypto, SupportedGroups);
            return supportedGroups;
        }
        private static SignatureAndHashAlgorithm CreateSignatureAlgorithm(int signatureScheme)
        {
            short hashAlgorithm = SignatureScheme.GetHashAlgorithm(signatureScheme);
            short signatureAlgorithm = SignatureScheme.GetSignatureAlgorithm(signatureScheme);
            return new SignatureAndHashAlgorithm(hashAlgorithm, signatureAlgorithm);
        }
    }
}
