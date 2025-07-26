using Domain;
using ExternalServices.Models;
using Refit;

namespace ExternalServices;

public interface IAsanFinance
{
    [Get("/OnlineAccount/User/GetUserFromAsanFinans?pin=5mdym0q&")]
    Task<Result<AsanFinanceResp>> GetAsanFinanceAsync([Query]string fin,[Query]string docNo);
}