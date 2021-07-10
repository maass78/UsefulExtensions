using System.Threading.Tasks;
using UsefulExtensions.SmsActivators.Types;

namespace UsefulExtensions.SmsActivators
{
    public interface ISmsActivator
    {
        decimal GetBalance();
        Task<decimal> GetBalanceAsync();

        Number GetNumber(string service, string country = null, string @operator = null);
        Task<Number> GetNumberAsync(string service, string country = null, string @operator = null);

        SetStatusResult SetStatus(int id, SetStatusEnum status);
        Task<SetStatusResult> SetStatusAsync(int id, SetStatusEnum status);

        Status GetStatus(int id);
        Task<Status> GetStatusAsync(int id);
    }
}
