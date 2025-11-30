using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Xpo;

namespace SenDev.Xaf.OneTimeMethods.Tests;

public class TestApplication : XafApplication
{
    public TestApplication()
    {
        Modules.Add(new TestModule());
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
        args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString);
    }
}
