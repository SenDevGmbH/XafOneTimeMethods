using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    public class EFCoreTestModule : ModuleBase
    {
        public EFCoreTestModule()
        {
            RequiredModuleTypes.Add(typeof(OneTimeMethodsModule));
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) => [new EFCoreTestUpdater(objectSpace, versionFromDB)];
    }
}
