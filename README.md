# SenDev.Xaf.OneTimeMethods

**SenDev.Xaf.OneTimeMethods** is a library for **DevExpress XAF (eXpressApp Framework)** applications using **XPO (eXpress Persistent Objects)**. It provides a robust mechanism to execute specific data manipulation or initialization methods **exactly once** during the application's database update lifecycle.

## Features

*   **One-Time Execution**: Ensure data migration or seeding logic runs only once per database.
*   **Execution Tracking**: Automatically logs executed methods in the database (`OneTimeMethodExecuteInfo`), recording execution time, duration, and database version.
*   **Flexible Timing**: Configure methods to run either `BeforeSchemaUpdate` or `AfterSchemaUpdate`.
*   **Easy Integration**: Simply inherit your module updater from `OneTimeMethodsModuleUpdaterBase` and decorate methods with `[OneTimeMethod]`.

## Requirements

*   .NET 8.0 or .NET Framework 4.7.2
*   DevExpress XAF (XPO)

## Installation

Install the NuGet package `SenDev.Xaf.OneTimeMethods.Xpo` to your XAF module project.

```bash
dotnet add package SenDev.Xaf.OneTimeMethods.Xpo
```

## Usage

### 1. Register the Module

In your module's constructor (e.g., `Module.cs`), add `OneTimeMethodsModule` to the `RequiredModuleTypes` collection.

```csharp
public sealed class MySolutionModule : ModuleBase {
    public MySolutionModule() {
        // ...
        RequiredModuleTypes.Add(typeof(SenDev.Xaf.OneTimeMethods.OneTimeMethodsModule));
    }
}
```

### 2. Inherit from `OneTimeMethodsModuleUpdaterBase`

Modify your `ModuleUpdater` class to inherit from `OneTimeMethodsModuleUpdaterBase` instead of the standard `ModuleUpdater`.

### 3. Define One-Time Methods

Create methods within your updater class and decorate them with the `[OneTimeMethod]` attribute. You can specify the execution sequence (Before or After schema update).

### Example

```csharp
using SenDev.Xaf.OneTimeMethods;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace MySolution.Module.DatabaseUpdate;

public class Updater : OneTimeMethodsModuleUpdaterBase
{
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }

    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();
        // Standard updater logic can still go here
    }

    [OneTimeMethod] // Defaults to AfterSchemaUpdate
    private void SeedInitialRoles()
    {
        // This code will run only once
        // Logic to create roles, users, or initial settings
    }

    [OneTimeMethod(OneTimeMethodExecutionSequence.BeforeSchemaUpdate)]
    private void CleanupLegacyData()
    {
        // This runs before the schema update
        // Useful for raw SQL commands or cleanup before XPO updates the schema
    }

    [OneTimeMethod(ConnectionType = typeof(Microsoft.Data.SqlClient.SqlConnection))]
    private void RunOnlyOnSqlServer()
    {
        // This code runs only if the underlying connection is a SQL Server connection
        // Useful for provider-specific SQL commands
    }
}
```

### Database Provider Filtering

You can restrict a one-time method to execute only for a specific database provider using the `ConnectionType` property. This is useful when you need to execute raw SQL commands that are specific to a database engine (e.g., T-SQL for SQL Server).

The method will execute only if the current connection object is assignable to the type specified in `ConnectionType`.

```csharp
[OneTimeMethod(ConnectionType = typeof(Microsoft.Data.SqlClient.SqlConnection))]
private void OptimizeIndexes()
{
    // Execute SQL Server specific index optimization
}
```

## How It Works

The library introduces a persistent class `OneTimeMethodExecuteInfo`. When the database update process runs:

1.  The base class scans for methods with the `[OneTimeMethod]` attribute in your updater.
2.  It queries the `OneTimeMethodExecuteInfo` table to check if the method has already been executed.
3.  If the method has not run, it is executed, and a new record is saved to `OneTimeMethodExecuteInfo` with the execution timestamp, duration, and database version.
