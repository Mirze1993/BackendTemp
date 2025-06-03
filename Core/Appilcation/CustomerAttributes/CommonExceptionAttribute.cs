using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Appilcation.CustomerAttributes;

public class CommonExceptionAttribute() : TypeFilterAttribute(typeof(CommonExceptionFilter));
public class CommonExceptionFilter(ILogger<CommonExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,"Common log exception");
        var result = new Result
        {
            Success = false, ErrorMessage = context.Exception.Message,
            Description = context.Exception switch
            {
                _ => ""
            }
        };
        context.Result = new ObjectResult(result);
        context.ExceptionHandled = true;
    }

}