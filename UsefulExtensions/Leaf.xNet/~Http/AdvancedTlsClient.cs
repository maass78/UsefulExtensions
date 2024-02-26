using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Org.BouncyCastle.Tls;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leaf.xNet
{
    public class AdvancedTlsClient : DefaultTlsClient
    {
		private class EmptyTlsAuthentication : TlsAuthentication
		{
			public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest) => null;
			public void NotifyServerCertificate(TlsServerCertificate serverCertificate) { }
		}

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

		public AdvancedTlsClient() : base(new BcTlsCrypto(new SecureRandom()))
		{
		}

		public override TlsAuthentication GetAuthentication() => new EmptyTlsAuthentication();
		protected override ProtocolVersion[] GetSupportedVersions() => SupportedVersions;
		protected override IList GetSupportedSignatureAlgorithms() => (IList)SignatureAlgorithms;
		protected override int[] GetSupportedCipherSuites() => SupportedCiphers;
		protected override IList GetSniServerNames() => _serverNames;
		protected override IList GetSupportedGroups(IList namedGroupRoles)
		{
			var supportedGroups = new ArrayList();
			TlsUtilities.AddIfSupported(supportedGroups, Crypto, SupportedGroups);
			return supportedGroups;
		}

		
	}
}
