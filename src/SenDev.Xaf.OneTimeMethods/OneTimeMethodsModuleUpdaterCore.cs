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

    /// <summary>
    /// Identifies this updater in the stored <see cref="IOneTimeMethodExecuteInfo"/> records, so that methods
    /// with the same name in different updaters are tracked separately. Defaults to the full name of the updater type.
    /// Override it to return a fixed value when you rename or move the updater class, otherwise its methods run again.
    /// </summary>
    protected virtual string UpdaterName => GetType().FullName!;

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

        // Records without an updater name were written before it was stored and count for every updater.
        var updaterName = UpdaterName;
        var executedMethods = ObjectSpace.GetObjects(ExecuteInfoType)
            .Cast<IOneTimeMethodExecuteInfo>()
            .Where(info => info.UpdaterName == null || info.UpdaterName == updaterName)
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
        info.UpdaterName = UpdaterName;
        info.ExecuteTime = startTime;
        info.ExecutedOnVersion = CurrentDBVersion.ToString();
        stopWatch.Stop();
        info.DurationInMilliseconds = stopWatch.ElapsedMilliseconds;
        ObjectSpace.CommitChanges();
    }
}
