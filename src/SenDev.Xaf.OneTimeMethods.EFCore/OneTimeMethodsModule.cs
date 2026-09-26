using DevExpress.ExpressApp;

namespace SenDev.Xaf.OneTimeMethods.EFCore;

public class OneTimeMethodsModule : ModuleBase
{
    public OneTimeMethodsModule()
    {
        AdditionalExportedTypes.Add(typeof(OneTimeMethodExecuteInfo));
    }
}
