using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    [Collection(XafTestCollection.Name)]
    public class EFCoreUpdaterNameTests : IDisposable
    {
        private readonly string sqliteFile = Path.Combine(Path.GetTempPath(), $"OneTimeMethods-{Guid.NewGuid():N}.db");
        private readonly EFCoreTestApplication application = new((builder, connectionString) => builder.UseSqlite(connectionString));

        public EFCoreUpdaterNameTests()
        {
            application.ConnectionString = $"Data Source={sqliteFile}";
            application.Setup();
            application.CheckCompatibility();
        }

        public void Dispose()
        {
            application.Dispose();
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(sqliteFile);
        }

        [Fact]
        public void StoresUpdaterFullClassName()
        {
            using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider");
            Assert.Equal(typeof(EFCoreTestUpdater).FullName, info.UpdaterName);
        }

        [Fact]
        public void SameMethodNameInAnotherUpdaterIsExecuted()
        {
            using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            new EFCoreRenamedTestUpdater(objectSpace, new Version()).UpdateDatabaseAfterUpdateSchema();

            var updaterNames = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>()
                .Where(i => i.MethodName == "RunOnAnyProvider")
                .Select(i => i.UpdaterName)
                .OrderBy(name => name)
                .ToArray();
            Assert.Equal([typeof(EFCoreTestUpdater).FullName, EFCoreRenamedTestUpdater.Name], updaterNames);
        }

        [Fact]
        public void RecordWithoutUpdaterNameCountsAsExecuted()
        {
            using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider").UpdaterName = null;
            objectSpace.CommitChanges();

            new EFCoreRenamedTestUpdater(objectSpace, new Version()).UpdateDatabaseAfterUpdateSchema();

            var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider");
            Assert.Null(info.UpdaterName);
        }
    }
}
