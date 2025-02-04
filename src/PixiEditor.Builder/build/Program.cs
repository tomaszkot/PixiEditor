using System.IO;
using Cake.Common.Build;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Path = System.IO.Path;

namespace PixiEditor.Cake.Builder;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string PathToProject { get; set; } = "../PixiEditor/PixiEditor.csproj";

    public string CrashReportWebhookUrl { get; set; }

    public string BackedUpConstants { get; set; }

    public string BuildConfiguration { get; set; } = "Release";

    public string OutputDirectory { get; set; } = "Builds";

    public string Runtime { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        bool hasWebhook = context.Arguments.HasArgument("crash-report-webhook-url");
        CrashReportWebhookUrl = hasWebhook
            ? context.Arguments.GetArgument("crash-report-webhook-url")
            : string.Empty;

        bool hasCustomProjectPath = context.Arguments.HasArgument("project-path");
        if (hasCustomProjectPath)
        {
            PathToProject = context.Arguments.GetArgument("project-path");
        }

        bool hasCustomConfiguration = context.Arguments.HasArgument("build-configuration");
        if (hasCustomConfiguration)
        {
            BuildConfiguration = context.Arguments.GetArgument("build-configuration");
        }

        bool hasCustomOutputDirectory = context.Arguments.HasArgument("o");
        if (hasCustomOutputDirectory)
        {
            OutputDirectory = context.Arguments.GetArgument("o");
        }

        Runtime = context.Arguments.GetArgument("runtime");
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(BuildProjectTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information("Built project successfully!");
    }
}

[TaskName("ReplaceSpecialStrings")]
public sealed class ReplaceSpecialStringsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information("Replacing special strings...");
        string projectPath = context.PathToProject;
        string filePath = Path.Combine(projectPath, "BuildConstants.cs");

        string result;
        var fileContent = File.ReadAllText(filePath);
        context.BackedUpConstants = fileContent;
        result = ReplaceSpecialStrings(context, fileContent);

        File.WriteAllText(filePath, result);
    }

    private string ReplaceSpecialStrings(BuildContext context, string fileContent)
    {
        string result = fileContent
            .Replace("${crash-report-webhook-url}", context.CrashReportWebhookUrl);

        return result;
    }
}

[TaskName("BuildProject")]
[IsDependentOn(typeof(ReplaceSpecialStringsTask))]
public sealed class BuildProjectTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information("Building project...");
        string projectPath = context.PathToProject;

        var settings = new DotNetPublishSettings()
        {
            Configuration = context.BuildConfiguration,
            SelfContained = false,
            Runtime = context.Runtime,
            OutputDirectory = context.OutputDirectory,
        };

        context.DotNetPublish(projectPath, settings);
    }

    public override void Finally(BuildContext context)
    {
        context.Log.Information("Cleaning up...");
        string constantsPath = Path.Combine(context.PathToProject, "BuildConstants.cs");

        File.WriteAllText(constantsPath, context.BackedUpConstants);
    }
}
