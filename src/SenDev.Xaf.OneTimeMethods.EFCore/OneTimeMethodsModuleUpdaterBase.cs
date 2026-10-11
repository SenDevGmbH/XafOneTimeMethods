using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EFCore;
using Microsoft.EntityFrameworkCore;

namespace SenDev.Xaf.OneTimeMethods.EFCore;

public abstract class OneTimeMethodsModuleUpdaterBase : OneTimeMethodsModuleUpdaterCore
{
    protected OneTimeMethodsModuleUpdaterBase(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }

    protected sealed override Type ExecuteInfoType => typeof(OneTimeMethodExecuteInfo);

    protected sealed override IDbConnection? GetConnection()
    {
        var database = (ObjectSpace as EFCoreObjectSpace)?.DbContext?.Database;
        return database != null && database.IsRelational() ? database.GetDbConnection() : null;
    }
}
