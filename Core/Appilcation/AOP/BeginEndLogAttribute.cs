using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Appilcation.AOP;

public class BeginEndLogAttribute(ILogger<BeginEndLogAttribute> logger) : MethodInterceptionBaseAttribute
{
    private readonly ILogger<BeginEndLogAttribute> _logger = logger;

    public override void Before()
    {
        logger.LogInformation("Begin  method");
        base.Before();
    }

    public override void After(object? o, MethodInfo info)
    {
        logger.LogInformation("end  method");
        base.After(o, info);
    }
}