using Xunit;

namespace SenDev.Xaf.OneTimeMethods.Tests;

// XAF keeps type information in a process-wide singleton, so tests that set up XAF applications must not run in parallel.
[CollectionDefinition(Name, DisableParallelization = true)]
public class XafTestCollection
{
    public const string Name = "XAF";
}
