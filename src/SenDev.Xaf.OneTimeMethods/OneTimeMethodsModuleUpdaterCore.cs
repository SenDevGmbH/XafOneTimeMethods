using System.Data;
using System.Diagnostics;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SenDev.Xaf.OneTimeMethods;

/// <summary>
/// ORM-independent part of the one-time methods updater. Use <c>OneTimeMethodsModuleUpdaterBase</c>
/// from the XPO or EF Core package as the base class of your module updater.
/// </summary>
public abstract class OneTimeMethodsModuleUpdaterCore : ModuleUpdater
{
    protected OneTimeMethodsModuleUpdaterCore(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }

    /// <summary>
    /// The persistent type that stores executed methods. It must implement <see cref="IOneTimeMethodExecuteInfo"/>.
    /// </summary>
    protected abstract Type ExecuteInfoType { get; }

    /// <summary>
    /// The database connection used by <see cref="ModuleUpdater.ObjectSpace"/>, or null if it cannot be determined.
    /// </summary>
    protected abstract IDbConnection? GetConnection();

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
        ExecuteOneTimeMethods(OneTimeMethodExecutionSequence.BeforeSchemaUpdate);
    }

    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();
        ExecuteOneTimeMethods(OneTimeMethodExecutionSequence.AfterSchemaUpdate);
    }

    private void ExecuteOneTimeMethods(OneTimeMethodExecutionSequence sequence)
    {
        var connection = GetConnection();

        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => new
            {
                MethodInfo = m,
                Attribute = m.GetCustomAttribute<OneTimeMethodAttribute>()
            })
            .Where(a => a.Attribute != null && a.Attribute.Sequence == sequence && IsProviderMatching(a.Attribute, connection))
            .ToArray();

        if (methods.Length == 0)
            return;

        var executedMethods = ObjectSpace.GetObjects(ExecuteInfoType)
            .Cast<IOneTimeMethodExecuteInfo>()
            .Select(info => info.MethodName)
            .ToHashSet();

        foreach (var method in methods.Where(m => !executedMethods.Contains(m.MethodInfo.Name)))
            ExecuteOneTimeMethod(method.MethodInfo);
    }

    private static bool IsProviderMatching(OneTimeMethodAttribute attribute, IDbConnection? connection)
    {
        if (attribute.ConnectionType == null)
        {
            return true;
        }

        if (connection == null)
        {
            return false;
        }

        return attribute.ConnectionType.IsAssignableFrom(connection.GetType());
    }

    private void ExecuteOneTimeMethod(MethodInfo method)
    {
        DateTime startTime = DateTime.Now;
        var stopWatch = Stopwatch.StartNew();
        method.Invoke(this, null);
        var info = (IOneTimeMethodExecuteInfo)ObjectSpace.CreateObject(ExecuteInfoType);
        info.MethodName = method.Name;
        info.ExecuteTime = startTime;
        info.ExecutedOnVersion = CurrentDBVersion.ToString();
        stopWatch.Stop();
        info.DurationInMilliseconds = stopWatch.ElapsedMilliseconds;
        ObjectSpace.CommitChanges();
    }
}
