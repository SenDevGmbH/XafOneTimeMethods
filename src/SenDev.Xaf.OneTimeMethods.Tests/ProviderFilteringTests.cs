using DevExpress.Xpo.DB;
using Testcontainers.MsSql;
using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests;

[Collection(XafTestCollection.Name)]
public class ProviderFilteringTests : IAsyncLifetime
{
    private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await msSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await msSqlContainer.DisposeAsync();
    }

    [Fact]
    public void CheckExecutedMSSqlMethod()
    {
    
        var connectionString = $"XPOProvider={MSSqlConnectionProvider.XpoProviderTypeString};{msSqlContainer.GetConnectionString()}";
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
