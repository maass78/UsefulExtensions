namespace UsefulExtensions.SmsActivators.Implementations
{
    public class Sim5Activator : SmsActivatorApiBase
    {
        protected override string ApiUrl => "http://api1.5sim.net/stubs/handler_api.php";

        public Sim5Activator(string apiKey) : base(apiKey)
        {
        }
    }
}