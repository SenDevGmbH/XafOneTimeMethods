using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    [Collection(XafTestCollection.Name)]
    public class EFCoreUpdaterNameTests : IDisposable
    {
        private readonly string _sqliteFile = Path.Combine(Path.GetTempPath(), $"OneTimeMethods-{Guid.NewGuid():N}.db");
        private readonly EFCoreTestApplication _application = new((builder, connectionString) => builder.UseSqlite(connectionString));

        public EFCoreUpdaterNameTests()
        {
            _application.ConnectionString = $"Data Source={_sqliteFile}";
            _application.Setup();
            _application.CheckCompatibility();
        }

        public void Dispose()
        {
            _application.Dispose();
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_sqliteFile);
        }

        [Fact]
        public void StoresUpdaterFullClassName()
        {
            using var objectSpace = _application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider");
            Assert.Equal(typeof(EFCoreTestUpdater).FullName, info.UpdaterName);
        }

        [Fact]
        public void SameMethodNameInAnotherUpdaterIsExecuted()
        {
            using var objectSpace = _application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
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
            using var objectSpace = _application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider").UpdaterName = null;
            objectSpace.CommitChanges();

            new EFCoreRenamedTestUpdater(objectSpace, new Version()).UpdateDatabaseAfterUpdateSchema();

            var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnAnyProvider");
            Assert.Null(info.UpdaterName);
        }
    }
}
