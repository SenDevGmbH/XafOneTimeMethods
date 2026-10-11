using DevExpress.Persistent.BaseImpl.EF;

namespace SenDev.Xaf.OneTimeMethods.EFCore;

/// <summary>
/// Stores an executed one-time method. Register it in your DbContext:
/// <c>public virtual DbSet&lt;OneTimeMethodExecuteInfo&gt; OneTimeMethodExecuteInfos { get; set; }</c>
/// </summary>
public class OneTimeMethodExecuteInfo : BaseObject, IOneTimeMethodExecuteInfo
{
    public virtual string? MethodName { get; set; }

    public virtual DateTime ExecuteTime { get; set; }

    public virtual string? ExecutedOnVersion { get; set; }

    public virtual double DurationInMilliseconds { get; set; }
}
