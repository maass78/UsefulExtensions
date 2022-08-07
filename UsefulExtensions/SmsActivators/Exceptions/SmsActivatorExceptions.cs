using System;

namespace UsefulExtensions.SmsActivators.Exceptions
{
    public class SmsActivatorException : Exception
    {
        public SmsActivatorException(string message) : base("Сервис смс-активаций вернул ответ: " + message)
        {
        }
    }

    public class SmsBadActionException : SmsActivatorException
    {
        public SmsBadActionException() : base("BAD_ACTION")
        {
        }
    }

    public class SmsBadKeyException : SmsActivatorException
    {
        public SmsBadKeyException() : base("BAD_KEY")
        {
        }
    }

    public class SmsNoBalanceException : SmsActivatorException
    {
        public SmsNoBalanceException() : base("NO_BALANCE")
        {
        }
    }

    public class SmsNoNumbersException : SmsActivatorException
    {
        public SmsNoNumbersException() : base("NO_NUMBERS")
        {
        }
    }
}