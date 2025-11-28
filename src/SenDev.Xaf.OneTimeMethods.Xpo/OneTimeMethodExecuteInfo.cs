using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SenDev.Xaf.OneTimeMethods;

public class OneTimeMethodExecuteInfo : BaseObject
{
    #region Methods

    public OneTimeMethodExecuteInfo(Session session) : base(session)
    {
    }

    #endregion

    #region Properties

    private string? methodName;
    public string? MethodName
    {
        get => methodName;
        set => SetPropertyValue(nameof(MethodName), ref methodName, value);
    }

    private DateTime executeTime;
    public DateTime ExecuteTime
    {
        get => executeTime;
        set => SetPropertyValue(nameof(ExecuteTime), ref executeTime, value);
    }

    private string? executedOnVersion;
    public string? ExecutedOnVersion
    {
        get => executedOnVersion;
        set => SetPropertyValue(nameof(ExecutedOnVersion), ref executedOnVersion, value);
    }

    private double durationInMilliseconds;
    public double DurationInMilliseconds
    {
        get => durationInMilliseconds;
        set => SetPropertyValue(nameof(DurationInMilliseconds), ref durationInMilliseconds, value);
    }

    #endregion
}