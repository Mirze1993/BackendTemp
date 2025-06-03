using System.Reflection;

namespace Appilcation.AOP;

public class BaseProxy<T> : DispatchProxy where T : class
{
    private T _target = null!;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod is null)
            throw new ArgumentException(nameof(targetMethod));

        var attributeList = Attribute.GetCustomAttributes(targetMethod, true);
        if (!attributeList.Any())
            return targetMethod.Invoke(_target, args);

        try
        {
            Before(attributeList);

            var result = targetMethod.Invoke(_target, args);

            if (result is Task)
            {
                var resultTask = result as Task;

                resultTask?.ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        OnException(task.Exception, attributeList);
                    }
                    else
                    {
                        object? trValue = null;
                        if (task.GetType().GetTypeInfo().IsGenericType &&
                            task.GetType().GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            var prop = task.GetType().GetTypeInfo().GetProperties()
                                .FirstOrDefault(mm => mm.Name == "Result");
                            if (prop != null) trValue = prop.GetValue(task);
                            After(targetMethod, attributeList, trValue);
                        }
                    }
                });
            }
            else
            {
                After(targetMethod, attributeList, result);
            }

            return result;
        }
        catch (Exception ex)
        {
            OnException(ex, attributeList);
            throw ex.InnerException ?? ex;
        }
    }

    private static void After(MethodInfo targetMethod, Attribute[] attributeList, object? result)
    {
        foreach (var attribute in attributeList)
            if (attribute is MethodInterceptionBaseAttribute baseAttribute)
                baseAttribute.After(result, targetMethod);
    }

    private static void OnException(Exception e, Attribute[] attributeList)
    {
        foreach (var attribute in attributeList)
            if (attribute is MethodInterceptionBaseAttribute baseAttribute)
                baseAttribute.Exception(e);
    }

    private static void Before(Attribute[] attributeList)
    {
        try
        {
            foreach (var attribute in attributeList)
                if (attribute is MethodInterceptionBaseAttribute baseAttribute)
                    baseAttribute.Before();
        }
        catch (Exception ex)
        {
            foreach (var attribute in attributeList)
                if (attribute is MethodInterceptionBaseAttribute baseAttribute)
                    baseAttribute.Exception(ex);
        }
    }

    public static T Create(T target)
    {
        // Create a new instance of the LoggingProxy<T> class
        object proxy = Create<T, BaseProxy<T>>();
        ((BaseProxy<T>)proxy).SetParameters(target);
        return (T)proxy;
    }

    private void SetParameters(T target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        _target = target;
    }
}