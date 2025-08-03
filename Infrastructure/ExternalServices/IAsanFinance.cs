using Domain;
using ExternalServices.Models;
using Refit;

namespace ExternalServices;

public interface IAsanFinance
{
    [Get("/OnlineAccount/User/GetUserFromAsanFinans")]
    Task<Result<AsanFinanceResp>> GetAsanFinanceAsync([Query]string pin,[Query]string docNo);
}