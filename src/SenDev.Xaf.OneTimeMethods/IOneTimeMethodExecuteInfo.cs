namespace SenDev.Xaf.OneTimeMethods;

/// <summary>
/// A persistent record of an executed one-time method. Implemented by the ORM-specific
/// <c>OneTimeMethodExecuteInfo</c> classes of the XPO and EF Core packages.
/// </summary>
public interface IOneTimeMethodExecuteInfo
{
    string? MethodName { get; set; }

    /// <summary>
    /// The <see cref="OneTimeMethodsModuleUpdaterCore.UpdaterName"/> of the updater that executed the method.
    /// Null for records written by versions that did not store it.
    /// </summary>
    string? UpdaterName { get; set; }

    DateTime ExecuteTime { get; set; }

    string? ExecutedOnVersion { get; set; }

    double DurationInMilliseconds { get; set; }
}
