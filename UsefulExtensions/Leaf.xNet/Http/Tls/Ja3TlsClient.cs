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
            TlsExtensionsUtilities.AddCompressCertificateExtension(clientExtensions, new int[2]);
            clientExtensions[ExtensionType.renegotiation_info] = TlsUtilities.EncodeOpaque8(TlsUtilities.EmptyBytes);
            clientExtensions[17513] = new byte[0];

            for (int i = 0; i < ExtensionsOrder.Length; i++)
            {
                if (!clientExtensions.ContainsKey(ExtensionsOrder[i]))
                {
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
