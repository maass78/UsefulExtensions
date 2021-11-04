namespace UsefulExtensions.Mail
{
    /// <summary>
    /// Класс-расширение для переноса MailKit прокси в LeafxNet прокси и обратно
    /// </summary>
    public static class ProxyConverter
    {
        /// <summary>
        /// Конвертирует LeafxNet HTTP прокси в MailKit HTTP прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static MailKit.Net.Proxy.HttpProxyClient ToMailKitProxy(this Leaf.xNet.HttpProxyClient proxy)
        {
            if (proxy == null)
                return null;

            return new MailKit.Net.Proxy.HttpProxyClient(proxy.Host, proxy.Port, new System.Net.NetworkCredential(proxy?.Username ?? "", proxy?.Password ?? ""));
        }

        /// <summary>
        /// Конвертирует LeafxNet Socks4a прокси в MailKit Socks4a прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static MailKit.Net.Proxy.Socks4aClient ToMailKitProxy(this Leaf.xNet.Socks4AProxyClient proxy)
        {
            if (proxy == null)
                return null;

            return new MailKit.Net.Proxy.Socks4aClient(proxy.Host, proxy.Port, new System.Net.NetworkCredential(proxy?.Username ?? "", proxy?.Password ?? ""));
        }

        /// <summary>
        /// Конвертирует LeafxNet Socks4 прокси в MailKit Socks4 прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static MailKit.Net.Proxy.Socks4Client ToMailKitProxy(this Leaf.xNet.Socks4ProxyClient proxy)
        {
            if (proxy == null)
                return null;

            return new MailKit.Net.Proxy.Socks4Client(proxy.Host, proxy.Port, new System.Net.NetworkCredential(proxy?.Username ?? "", proxy?.Password ?? ""));
        }

        /// <summary>
        /// Конвертирует LeafxNet Socks5 прокси в MailKit Socks4 прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static MailKit.Net.Proxy.Socks5Client ToMailKitProxy(this Leaf.xNet.Socks5ProxyClient proxy)
        {
            if (proxy == null)
                return null;

            return new MailKit.Net.Proxy.Socks5Client(proxy.Host, proxy.Port, new System.Net.NetworkCredential(proxy?.Username ?? "", proxy?.Password ?? ""));
        }

        /// <summary>
        /// Конвертирует MailKit HTTP прокси в LeafxNet HTTP прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static Leaf.xNet.HttpProxyClient ToLeafxNetProxy(this MailKit.Net.Proxy.HttpProxyClient proxy)
        {
            if (proxy == null)
                return null;

            return new Leaf.xNet.HttpProxyClient(proxy.ProxyHost, proxy.ProxyPort, proxy?.ProxyCredentials?.UserName ?? "", proxy?.ProxyCredentials?.Password ?? "");
        }

        /// <summary>
        /// Конвертирует MailKit Socks4a прокси в LeafxNet Socks4a прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static Leaf.xNet.Socks4AProxyClient ToLeafxNetProxy(this MailKit.Net.Proxy.Socks4aClient proxy)
        {
            if (proxy == null)
                return null;

            return new Leaf.xNet.Socks4AProxyClient(proxy.ProxyHost, proxy.ProxyPort) { Username = proxy?.ProxyCredentials?.UserName ?? "", Password = proxy?.ProxyCredentials?.Password ?? "" };
        }
        /// <summary>
        /// Конвертирует MailKit Socks4 прокси в LeafxNet Socks4 прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static Leaf.xNet.Socks4ProxyClient ToLeafxNetProxy(this MailKit.Net.Proxy.Socks4Client proxy)
        {
            if (proxy == null)
                return null;

            return new Leaf.xNet.Socks4ProxyClient(proxy.ProxyHost, proxy.ProxyPort) { Username = proxy?.ProxyCredentials?.UserName ?? "", Password = proxy?.ProxyCredentials?.Password ?? "" };
        }
        /// <summary>
        /// Конвертирует MailKit Socks5 прокси в LeafxNet Socks5 прокси
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static Leaf.xNet.Socks5ProxyClient ToLeafxNetProxy(this MailKit.Net.Proxy.Socks5Client proxy)
        {
            if (proxy == null)
                return null;

            return new Leaf.xNet.Socks5ProxyClient(proxy.ProxyHost, proxy.ProxyPort, proxy?.ProxyCredentials?.UserName ?? "", proxy?.ProxyCredentials?.Password ?? "");
        }
    }
}
