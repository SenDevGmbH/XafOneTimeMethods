# This is a test for PR
# SenDev.Xaf.OneTimeMethods

**SenDev.Xaf.OneTimeMethods** is a library for **DevExpress XAF (eXpressApp Framework)** applications using **XPO (eXpress Persistent Objects)**. It provides a robust mechanism to execute specific data manipulation or initialization methods **exactly once** during the application's database update lifecycle.

## Features

*   **One-Time Execution**: Ensure data migration or seeding logic runs only once per database.
*   **Execution Tracking**: Automatically logs executed methods in the database (`OneTimeMethodExecuteInfo`), recording execution time, duration, and database version.
*   **Flexible Timing**: Configure methods to run either `BeforeSchemaUpdate` or `AfterSchemaUpdate`.
*   **Easy Integration**: Simply inherit your module updater from `OneTimeMethodsModuleUpdaterBase` and decorate methods with `[OneTimeMethod]`.

## Requirements

*   .NET 9.0
*   DevExpress XAF (XPO)

## Installation

Install the NuGet package `SenDev.Xaf.OneTimeMethods.Xpo` to your XAF module project.

```bash
dotnet add package SenDev.Xaf.OneTimeMethods.Xpo
```

## Usage

### 1. Inherit from `OneTimeMethodsModuleUpdaterBase`

Modify your `ModuleUpdater` class to inherit from `OneTimeMethodsModuleUpdaterBase` instead of the standard `ModuleUpdater`.

### 2. Define One-Time Methods

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
}
```

## How It Works

The library introduces a persistent class `OneTimeMethodExecuteInfo`. When the database update process runs:

1.  The base class scans for methods with the `[OneTimeMethod]` attribute in your updater.
2.  It queries the `OneTimeMethodExecuteInfo` table to check if the method has already been executed.
3.  If the method has not run, it is executed, and a new record is saved to `OneTimeMethodExecuteInfo` with the execution timestamp, duration, and database version.
