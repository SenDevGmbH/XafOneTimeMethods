using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests;

[Collection(XafTestCollection.Name)]
public class UpdaterNameTests : IDisposable
{
    private readonly string sqliteFile = Path.Combine(Path.GetTempPath(), $"OneTimeMethods-{Guid.NewGuid():N}.db");
    private readonly TestApplication application = new();

    public UpdaterNameTests()
    {
        SQLitePCL.Batteries.Init();
        application.ConnectionString = $"XPOProvider=SQLite;Data Source={sqliteFile}";
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
        var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnSQLite");
        Assert.Equal(typeof(TestUpdater).FullName, info.UpdaterName);
    }

    [Fact]
    public void SameMethodNameInAnotherUpdaterIsExecuted()
    {
        using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
        new RenamedTestUpdater(objectSpace, new Version()).UpdateDatabaseAfterUpdateSchema();

        var updaterNames = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>()
            .Where(i => i.MethodName == "RunOnSQLite")
            .Select(i => i.UpdaterName)
            .OrderBy(name => name)
            .ToArray();
        Assert.Equal([RenamedTestUpdater.Name, typeof(TestUpdater).FullName], updaterNames);
    }

    [Fact]
    public void RecordWithoutUpdaterNameCountsAsExecuted()
    {
        using var objectSpace = application.CreateObjectSpace(typeof(OneTimeMethodExecuteInfo));
        objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnSQLite").UpdaterName = null;
        objectSpace.CommitChanges();

        new RenamedTestUpdater(objectSpace, new Version()).UpdateDatabaseAfterUpdateSchema();

        var info = objectSpace.GetObjectsQuery<OneTimeMethodExecuteInfo>().Single(i => i.MethodName == "RunOnSQLite");
        Assert.Null(info.UpdaterName);
    }
}
