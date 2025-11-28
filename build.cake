var target = Argument("target", "Pack");
var buildNumber = Argument("buildNumber", "1");
var configuration = Argument("configuration", "Release");

// Define the list of DevExpress versions to build against
var devExpressVersions = new List<string>
{
    "24.1.7",
    "25.1.4.55"
};

var artifactsDir = "./artifacts";

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("Restore")
    .Does(() =>
{
    // Restore is handled inside the loop to ensure correct package versions
});

Task("Build")
    .Does(() =>
{
    // Build is handled inside the loop
});

Task("Pack")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var version in devExpressVersions)
    {
        Information($"Processing DevExpress Version: {version}");

        string packageVersion;

        // Check if it's a self-compiled version (4 segments)
        var versionSegments = version.Split('.');
        if (versionSegments.Length == 4)
        {
            // Self-compiled logic: 25.1.3.10 -> 25.1.3.10001
            if (int.TryParse(versionSegments[3], out int lastSegment))
            {
                int newLastSegment = (lastSegment * 1000) + int.Parse(buildNumber);
                packageVersion = $"{versionSegments[0]}.{versionSegments[1]}.{versionSegments[2]}.{newLastSegment}";
            }
            else
            {
                throw new InvalidOperationException($"Invalid version format for self-compiled version: {version}");
            }
        }
        else
        {
            // Standard version logic: 25.1.3 -> 25.1.3.1
            packageVersion = $"{version}.{buildNumber}";
        }

        Information($"Calculated Package Version: {packageVersion}");

        var msBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("DevExpressPackageVersion", version)
            .WithProperty("PackageVersion", packageVersion);

        var nugetSettings = GetNuGetSettings(version);

        // Restore
        DotNetRestore("./src/SenDev.Xaf.OneTimeMethods.slnx", new DotNetRestoreSettings
        {
            MSBuildSettings = msBuildSettings,
            Sources = new [] { nugetSettings.Source }
        });

        // Build
        DotNetBuild("./src/SenDev.Xaf.OneTimeMethods.slnx", new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true,
            MSBuildSettings = msBuildSettings
        });

        // Pack
        DotNetPack("./src/SenDev.Xaf.OneTimeMethods.slnx", new DotNetPackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = artifactsDir,
            MSBuildSettings = msBuildSettings
        });
    }
});

Task("Push")
    .IsDependentOn("Pack")
    .Does(() =>
{
    foreach (var version in devExpressVersions)
    {
        Information($"Processing Push for DevExpress Version: {version}");

        string packageVersion;
        
        // Re-calculate version logic to find the file
        var versionSegments = version.Split('.');
        if (versionSegments.Length == 4)
        {
            if (int.TryParse(versionSegments[3], out int lastSegment))
            {
                int newLastSegment = (lastSegment * 1000) + int.Parse(buildNumber);
                packageVersion = $"{versionSegments[0]}.{versionSegments[1]}.{versionSegments[2]}.{newLastSegment}";
            }
            else
            {
                throw new InvalidOperationException($"Invalid version format for self-compiled version: {version}");
            }
        }
        else
        {
            packageVersion = $"{version}.{buildNumber}";
        }

        var packagePath = $"{artifactsDir}/SenDev.Xaf.OneTimeMethods.Xpo.{packageVersion}.nupkg";
        
        if (!FileExists(packagePath))
        {
            Warning($"Package not found: {packagePath}");
            continue;
        }

        var nugetSettings = GetNuGetSettings(version);

        if (string.IsNullOrEmpty(nugetSettings.ApiKey))
        {
             Warning($"API Key not found for version {version}. Skipping push.");
             continue;
        }

        Information($"Pushing {packagePath} to {nugetSettings.Source}...");
        DotNetNuGetPush(packagePath, new DotNetNuGetPushSettings
        {
            Source = nugetSettings.Source,
            ApiKey = nugetSettings.ApiKey,
            SkipDuplicate = true
        });

    }
});

RunTarget(target);

public (string Source, string ApiKey) GetNuGetSettings(string version)
{
    var nugetApiKey = EnvironmentVariable("NUGET_API_KEY");
    var nugetSource = "https://api.nuget.org/v3/index.json";

    var azureApiKey = EnvironmentVariable("AZURE_NUGET_KEY");
    var azureSource = EnvironmentVariable("AZURE_NUGET_SOURCE");

    var versionSegments = version.Split('.');
    if (versionSegments.Length == 4)
    {
        return (azureSource, azureApiKey);
    }
    
    return (nugetSource, nugetApiKey);
}
