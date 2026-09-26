using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    [Collection(XafTestCollection.Name)]
    public class EFCoreProviderFilteringTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
        private readonly string _sqliteFile = Path.Combine(Path.GetTempPath(), $"OneTimeMethods-{Guid.NewGuid():N}.db");

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.DisposeAsync();
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_sqliteFile);
        }

        [Fact]
        public void CheckExecutedMSSqlMethod()
        {
            var executedMethods = RunUpdater(_msSqlContainer.GetConnectionString(), (builder, connectionString) => builder.UseSqlServer(connectionString));
            Assert.Equal(["RunBeforeSchemaUpdate", "RunOnAnyProvider", "RunOnMssql"], executedMethods);
        }

        [Fact]
        public void CheckSQLiteMethodExecuted()
        {
            var executedMethods = RunUpdater($"Data Source={_sqliteFile}", (builder, connectionString) => builder.UseSqlite(connectionString));
            Assert.Equal(["RunBeforeSchemaUpdate", "RunOnAnyProvider", "RunOnSQLite"], executedMethods);
        }

        private static string?[] RunUpdater(string connectionString, Action<DbContextOptionsBuilder, string> configureProvider)
        {
            using var application = new EFCoreTestApplication(configureProvider);
            application.ConnectionString = connectionString;
            application.Setup();
            application.CheckCompatibility();
            using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
            return objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>()
                .Select(info => info.MethodName)
                .OrderBy(name => name)
                .ToArray();
        }
    }
}
