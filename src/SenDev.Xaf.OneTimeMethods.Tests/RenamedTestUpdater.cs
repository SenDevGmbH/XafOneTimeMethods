using DevExpress.ExpressApp;

namespace SenDev.Xaf.OneTimeMethods.Tests;

/// <summary>
/// Has a method with the same name as <see cref="TestUpdater"/> and a fixed updater name.
/// </summary>
public class RenamedTestUpdater : OneTimeMethodsModuleUpdaterBase
{
    public const string Name = "SenDev.Xaf.OneTimeMethods.Tests.OldUpdaterName";

    public RenamedTestUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion)
    {
    }

    protected override string UpdaterName => Name;

    [OneTimeMethod]
    private void RunOnSQLite()
    {
    }
}
