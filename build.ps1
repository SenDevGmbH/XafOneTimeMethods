[CmdletBinding()]
Param(
    [string]$target = "Pack",
    [string]$configuration = "Release",
    [string]$buildNumber = "1",
    [string]$verbosity = "Normal"
)


# Restore dotnet tools
dotnet tool restore

# Run Cake
dotnet cake build.cake --target=$target --configuration=$configuration --buildNumber=$buildNumber --verbosity=$verbosity
