namespace UsefulExtensions.SmsActivators.Types
{
	public class Status
	{
		public StatusEnum StatusEnum { get; set; }

		public string SmsCode { get; set; }

		public Status(StatusEnum statusEnum, string smscode)
		{
			StatusEnum = statusEnum;
			SmsCode = smscode;
		}
	}
}
