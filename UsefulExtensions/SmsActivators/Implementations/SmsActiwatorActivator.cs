namespace UsefulExtensions.SmsActivators.Implementations
{
    internal class SmsActiwatorActivator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "https://sms-acktiwator.ru/stubs/handler_api.php";

        public SmsActiwatorActivator(string apiKey) : base(apiKey) { }
    }
}
