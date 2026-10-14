using DevExpress.ExpressApp;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    /// <summary>
    /// Has a method with the same name as <see cref="EFCoreTestUpdater"/> and a fixed updater name.
    /// </summary>
    public class EFCoreRenamedTestUpdater : OneTimeMethodsModuleUpdaterBase
    {
        public const string Name = "SenDev.Xaf.OneTimeMethods.Tests.EFCore.OldUpdaterName";

        public EFCoreRenamedTestUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
        {
        }

        protected override string UpdaterName => Name;

        [OneTimeMethod]
        private void RunOnAnyProvider()
        {
        }
    }
}
