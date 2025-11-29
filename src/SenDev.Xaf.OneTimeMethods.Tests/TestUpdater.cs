using DevExpress.ExpressApp;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace SenDev.Xaf.OneTimeMethods.Tests;

public class TestUpdater : OneTimeMethodsModuleUpdaterBase
{
    public TestUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }

    [OneTimeMethod(ConnectionType = typeof(SqlConnection))]
    private void RunOnMssql()
    {
    }

    [OneTimeMethod(ConnectionType = typeof(SqliteConnection))]
    private void RunOnSQLite()
    {
    }
}
