using System.Diagnostics;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SenDev.Xaf.OneTimeMethods;

public abstract class OneTimeMethodsModuleUpdaterBase : ModuleUpdater
{
    protected OneTimeMethodsModuleUpdaterBase(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }


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
        var executedMethods = ObjectSpace.GetObjects<OneTimeMethodExecuteInfo>();
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => new
            {
                MethodInfo = m,
                Attribute = m.GetCustomAttribute<OneTimeMethodAttribute>()
            })
            .Where(a => a.Attribute != null && a.Attribute.Sequence == sequence && !executedMethods.Any(em => em.MethodName == a.MethodInfo.Name))
            .ToArray();

        foreach (var method in methods)
            ExecuteOneTimeMethod(method.MethodInfo);
    }

    private void ExecuteOneTimeMethod(MethodInfo method)
    {
        DateTime startTime = DateTime.Now;
        var stopWatch = Stopwatch.StartNew();
        method.Invoke(this, null);
        var info = ObjectSpace.CreateObject<OneTimeMethodExecuteInfo>();
        info.MethodName = method.Name;
        info.ExecuteTime = startTime;
        info.ExecutedOnVersion = CurrentDBVersion.ToString();
        stopWatch.Stop();
        info.DurationInMilliseconds = stopWatch.ElapsedMilliseconds;
        ObjectSpace.CommitChanges();
    }

}