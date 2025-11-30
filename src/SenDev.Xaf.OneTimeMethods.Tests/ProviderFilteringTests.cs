using DevExpress.Xpo.DB;
using Testcontainers.MsSql;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests;

public class ProviderFilteringTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    [Fact]
    public void CheckExecutedMSSqlMethod()
    {
    
        var connectionString = $"XPOProvider={MSSqlConnectionProvider.XpoProviderTypeString};{_msSqlContainer.GetConnectionString()}";
        AssertMethodExecuted(connectionString, "RunOnMssql");
    }

    [Fact]
    public void CheckSQLiteMethodExecuted()
    {
        SQLitePCL.Batteries.Init();
        var connectionString = "XPOProvider=SQLite;Data Source=:memory:;filename=test";
        AssertMethodExecuted(connectionString, "RunOnSQLite");
    }
    private static void AssertMethodExecuted(string connectionString, string methodName)
    {
        var application = new TestApplication();
        application.ConnectionString = connectionString;
        application.Setup();
        application.CheckCompatibility();
        using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
        var executionInfo = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(info => info.MethodName == methodName);
        Assert.NotNull(executionInfo);
    }
    
}
