using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace SenDev.Xaf.OneTimeMethods;

public abstract class OneTimeMethodsModuleUpdaterBase : OneTimeMethodsModuleUpdaterCore
{
    protected OneTimeMethodsModuleUpdaterBase(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }

    protected sealed override Type ExecuteInfoType => typeof(OneTimeMethodExecuteInfo);

    protected sealed override IDbConnection? GetConnection() => (ObjectSpace as XPObjectSpace)?.Session?.DataLayer?.Connection;
}
