#tool "nuget:?package=GitVersion.CommandLine&version=3.6.5"
#tool "nuget:?package=NUnit.Runners&version=2.6.4"

var CONFIGURATION = Argument<string>("c", "Release");

string NUGET_FEED() {
    return EnvironmentVariableOrFail("NUGET_FEED");
}

var src = Directory("./src");
var dst = Directory("./artifacts");
var test = Directory("./test");
var packages = dst + Directory("./packages");

var currentGitVersion = new Lazy<GitVersion>(
    () => {
        var settings = new GitVersionSettings {
        UpdateAssemblyInfo = true,
        UpdateAssemblyInfoFilePath = src + File("SharedAssemblyInfo.cs"),
        OutputType = GitVersionOutput.Json,
        NoFetch = true
    };

    return GitVersion(settings);
    }
);

string EnvironmentVariableOrFail(string varName){
    return EnvironmentVariable(varName) ?? throw new Exception($"Can't find variable {varName}");
}

IEnumerable<FilePath> GetProjectFiles()
{
    return GetFiles(src.Path + "/*/*.csproj").Where(file=>
        !file.GetFilenameWithoutExtension().FullPath.EndsWith("Tests")).OrderBy(x=>x.FullPath);
}

bool IsDotNetStandard(FilePath project)
{
    return System.IO.File.ReadAllText(project.FullPath).Contains("<Project Sdk=\"Microsoft.NET.Sdk\">");
}

Task("Clean").Does(() => {
    CleanDirectories(dst);
    CleanDirectories(src.Path + "/packages");
    CleanDirectories(src.Path + "/**/bin");
    CleanDirectories(src.Path + "/**/obj");
    CleanDirectories(src.Path + "/**/pkg");
    CleanDirectories(test.Path + "/**/bin");
    CleanDirectories(test.Path + "/**/obj");
});

Task("Restore").Does(() => {
    EnsureDirectoryExists(dst);
    EnsureDirectoryExists(packages);

    foreach(var sln in GetFiles("*.sln")) {
        NuGetRestore(sln);
    }
});

Task("SemVer").Does(() => {
    var version = currentGitVersion.Value;

    Information("{{  FullSemVer: {0}", version.FullSemVer);
    Information("    NuGetVersionV2: {0}", version.NuGetVersionV2);
    Information("    InformationalVersion: {0}  }}", version.InformationalVersion);
});

Task("Build").Does(() => {
    var settings = new DotNetCoreBuildSettings {
        Configuration = CONFIGURATION,
        Verbosity = DotNetCoreVerbosity.Normal
    };
    foreach(var sln in GetFiles("*.sln")) {
        DotNetCoreBuild(sln.FullPath, settings);
    }
});

Task("Test").Does(() => {
    Information("Running unit tests...");
    foreach(var project in GetFiles(test.Path + "/**/*.csproj"))
    {
        DotNetCoreTest(
            project.FullPath,
            new DotNetCoreTestSettings()
            {
                Configuration = CONFIGURATION,
                NoBuild = false
            });
    }
});

Task("Pack").Does(() => {
    var gitVersion = currentGitVersion.Value;

    var msBuildSettings
        = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", gitVersion.NuGetVersionV2);

    var coreSettings = new DotNetCorePackSettings {
        Configuration = CONFIGURATION,
        OutputDirectory = packages,
        MSBuildSettings = msBuildSettings,
        IncludeSymbols = true
    };

	foreach(var file in GetProjectFiles().Where(file=>IsDotNetStandard(file))) {
		DotNetCorePack(file.ToString(), coreSettings);
	}

    var settings = new NuGetPackSettings {
        Symbols = true,
        IncludeReferencedProjects = false,
        Verbosity = NuGetVerbosity.Detailed,
        Properties = new Dictionary<string, string> {
            { "Configuration", CONFIGURATION }
        },
        OutputDirectory = packages
    };

    NuGetPack(GetProjectFiles().Where(file => !IsDotNetStandard(file)), settings);
});

Task("Push").Does(() => {
    var settings = new DotNetCoreNuGetPushSettings {
         Source = NUGET_FEED()
     };

    foreach(var package in GetFiles(packages.Path + "/*.nupkg").Where(p => !p.FullPath.Contains(".symbols."))) {
        DotNetCoreNuGetPush(package.ToString(), settings);
    }
});

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("SemVer")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

Task("BuildServer")
  .IsDependentOn("Default")
  .IsDependentOn("Push");

RunTarget(Argument("target", "Default"));
