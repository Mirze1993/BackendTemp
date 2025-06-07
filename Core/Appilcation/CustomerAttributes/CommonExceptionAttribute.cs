using System.Data.Common;
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
            Success = false, 
            ErrorMessage =context.Exception switch
            {
                DbException e => Translate( e),
                _ =>context.Exception.Message
            },
            Description = context.Exception switch
            {
                _ => ""
            }
        };
        context.Result = new ObjectResult(result);
        context.ExceptionHandled = true;
    }

    private static string Translate(DbException ex)
    {
        return ex.Message switch
        {
            { } msg when msg.Contains("ORA-20002") => "Data already exists",
            { } msg when msg.Contains("ORA-00001") => "Unique constraint violated",
            _ => "Unknown error"
        };
    }

}