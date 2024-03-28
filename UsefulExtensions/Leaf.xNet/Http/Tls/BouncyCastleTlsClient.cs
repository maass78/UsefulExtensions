using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Org.BouncyCastle.Tls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leaf.xNet
{
	/// <summary>
	/// Обычный TLS клиент
	/// </summary>
    public class BouncyCastleTlsClient : DefaultTlsClient
    {
		private ServerName[] _serverNames;
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

		public IList<SignatureAndHashAlgorithm> SignatureAlgorithms { get; set; }

		public int[] SupportedCiphers { get; set; }

		public int[] SupportedGroups { get; set; }
        
        public ProtocolVersion[] SupportedVersions { get; set; }

		public BouncyCastleTlsClient() : base(new BcTlsCrypto(new SecureRandom()))
		{
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
    }
}
