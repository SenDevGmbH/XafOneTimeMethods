using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ReportServer.ServiceModel.ConnectionProviders;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;

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
        var connection = (ObjectSpace as XPObjectSpace)?.Session?.DataLayer?.Connection;   

        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => new
            {
                MethodInfo = m,
                Attribute = m.GetCustomAttribute<OneTimeMethodAttribute>()
            })
            .Where(a => a.Attribute != null && a.Attribute.Sequence == sequence && !executedMethods.Any(em => em.MethodName == a.MethodInfo.Name)
                && IsProviderMatching(a.Attribute, connection))
            .ToArray();

        foreach (var method in methods)
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
        var info = ObjectSpace.CreateObject<OneTimeMethodExecuteInfo>();
        info.MethodName = method.Name;
        info.ExecuteTime = startTime;
        info.ExecutedOnVersion = CurrentDBVersion.ToString();
        stopWatch.Stop();
        info.DurationInMilliseconds = stopWatch.ElapsedMilliseconds;
        ObjectSpace.CommitChanges();
    }

}