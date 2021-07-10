namespace UsefulExtensions.SmsActivators.Implementations
{
    public class VakSmsComActivator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "https://vak-sms.com/stubs/handler_api.php";
        public VakSmsComActivator(string apiKey) : base(apiKey) { }
    }
}
