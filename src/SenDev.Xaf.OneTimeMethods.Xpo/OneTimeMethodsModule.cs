using DevExpress.ExpressApp;

namespace SenDev.Xaf.OneTimeMethods;

public class OneTimeMethodsModule : ModuleBase
{
    public OneTimeMethodsModule()
    {
        AdditionalExportedTypes.Add(typeof(OneTimeMethodExecuteInfo));
    }
}