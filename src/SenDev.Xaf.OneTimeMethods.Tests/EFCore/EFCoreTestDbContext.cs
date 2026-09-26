using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;

namespace SenDev.Xaf.OneTimeMethods.Tests.EFCore
{
    using SenDev.Xaf.OneTimeMethods.EFCore;

    public class EFCoreTestDbContext : DbContext
    {
        public EFCoreTestDbContext(DbContextOptions<EFCoreTestDbContext> options) : base(options)
        {
        }

        public DbSet<ModuleInfo> ModulesInfo { get; set; } = null!;

        public DbSet<OneTimeMethodExecuteInfo> OneTimeMethodExecuteInfos { get; set; } = null!;
    }
}
