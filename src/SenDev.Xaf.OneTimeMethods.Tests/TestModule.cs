using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SenDev.Xaf.OneTimeMethods.Tests;

public class TestModule : ModuleBase
{
    public TestModule()
    {
        RequiredModuleTypes.Add(typeof(OneTimeMethodsModule));
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) => [new TestUpdater(objectSpace, versionFromDB)];


}
