using Org.BouncyCastle.Tls;

namespace Leaf.xNet
{
    public class EmptyTlsAuthentication : TlsAuthentication
    {
        public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest) => null;
        public void NotifyServerCertificate(TlsServerCertificate serverCertificate) { }
    }
}
