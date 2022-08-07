namespace UsefulExtensions.SmsActivators.Implementations
{
    public class SmsHubActivator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "https://smshub.org/stubs/handler_api.php";

        public SmsHubActivator(string apiKey) : base(apiKey)
        {
        }
    }
}