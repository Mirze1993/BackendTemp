using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Appilcation.AOP;

public class ExcecuteTimeAttribute : MethodInterceptionBaseAttribute
{
    private Stopwatch? _stopwatch;


    public override void Before()
    {
        _stopwatch = Stopwatch.StartNew();
        base.Before();
    }

    public override void After(object? o, MethodInfo info)
    {
        _stopwatch?.Stop();
        var a = JsonSerializer.Serialize(o);
        Console.WriteLine(JsonSerializer.Serialize(o));
        Console.WriteLine(
            "MeasureDurationInterceptor: {0} executed in {1} milliseconds.",
            info.Name,
            _stopwatch?.Elapsed.TotalMilliseconds.ToString("0.000")
        );
        base.After(o, info);
    }

    public override void Exception(Exception e)
    {
        _stopwatch?.Stop();

        Console.WriteLine(
            @"MeasureDurationInterceptor:  executed in {0} mill*********iseconds.",
            _stopwatch?.Elapsed.TotalMilliseconds.ToString("0.000")
        );
        base.Exception(e);
    }
}