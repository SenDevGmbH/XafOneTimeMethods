using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    [Collection(XafTestCollection.Name)]
    public class EFCoreProviderFilteringTests : IAsyncLifetime
    {
        private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder().Build();
        private readonly string sqliteFile = Path.Combine(Path.GetTempPath(), $"OneTimeMethods-{Guid.NewGuid():N}.db");

        public async Task InitializeAsync()
        {
            await msSqlContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await msSqlContainer.DisposeAsync();
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(sqliteFile);
        }

        [Fact]
        public void CheckExecutedMSSqlMethod()
        {
            var executedMethods = RunUpdater(msSqlContainer.GetConnectionString(), (builder, connectionString) => builder.UseSqlServer(connectionString));
            Assert.Equal(["RunBeforeSchemaUpdate", "RunOnAnyProvider", "RunOnMssql"], executedMethods);
        }

        [Fact]
        public void CheckSQLiteMethodExecuted()
        {
            var executedMethods = RunUpdater($"Data Source={sqliteFile}", (builder, connectionString) => builder.UseSqlite(connectionString));
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
