using System.Reflection;

namespace Appilcation.AOP;

public abstract class MethodInterceptionBaseAttribute : Attribute
{
    public virtual void Before()
    {
    }

    public virtual void After(object? o, MethodInfo info)
    {
    }

    public virtual void Exception(Exception e)
    {
    }
}