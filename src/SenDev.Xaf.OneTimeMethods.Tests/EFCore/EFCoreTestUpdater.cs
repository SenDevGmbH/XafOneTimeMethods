using DevExpress.ExpressApp;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    public class EFCoreTestUpdater : OneTimeMethodsModuleUpdaterBase
    {
        public EFCoreTestUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
        {
        }

        [OneTimeMethod(OneTimeMethodExecutionSequence.BeforeSchemaUpdate)]
        private void RunBeforeSchemaUpdate()
        {
        }

        [OneTimeMethod]
        private void RunOnAnyProvider()
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
}
