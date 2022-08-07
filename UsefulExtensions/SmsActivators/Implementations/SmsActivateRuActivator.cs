namespace UsefulExtensions.SmsActivators.Implementations
{
    public class SmsActivateRuActivator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "https://sms-activate.ru/stubs/handler_api.php";

        public SmsActivateRuActivator(string apiKey) : base(apiKey)
        {
        }
    }
}