namespace UsefulExtensions.SmsActivators.Types
{
    public class Number
    {
        public int Id { get; private set; }
        public string PhoneNumber { get; private set; }

        public Number(int id, string phoneNumber)
        {
            Id = id;
            PhoneNumber = phoneNumber;
        }
    }
}