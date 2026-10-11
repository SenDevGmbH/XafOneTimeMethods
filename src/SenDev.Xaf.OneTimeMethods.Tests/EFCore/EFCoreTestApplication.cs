using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EFCore;
using DevExpress.ExpressApp.Layout;
using Microsoft.EntityFrameworkCore;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore;

public class EFCoreTestApplication : XafApplication
{
    private readonly Action<DbContextOptionsBuilder, string> configureProvider;

    public EFCoreTestApplication(Action<DbContextOptionsBuilder, string> configureProvider)
    {
        this.configureProvider = configureProvider;
        Modules.Add(new EFCoreTestModule());
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
    }

    protected override LayoutManager CreateLayoutManagerCore(bool simple)
    {
        return new TestLayoutManager();
    }

    protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args)
    {
        args.Updater.Update();
        args.Handled = true;
    }

    protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
    {
        args.ObjectSpaceProvider = new EFCoreObjectSpaceProvider<EFCoreTestDbContext>(TypesInfo, args.ConnectionString,
            (builder, connectionString) => configureProvider(builder, connectionString));
    }
}
