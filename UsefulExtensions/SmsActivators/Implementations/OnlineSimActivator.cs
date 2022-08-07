namespace UsefulExtensions.SmsActivators.Implementations
{
    public class OnlineSimActivator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "http://api-conserver.onlinesim.ru/stubs/handler_api.php";

        public OnlineSimActivator(string apiKey) : base(apiKey)
        {
        }
    }
}