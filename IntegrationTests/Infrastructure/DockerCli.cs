using System.Diagnostics;

namespace IntegrationTests.Infrastructure;

internal static class DockerCli
{
    /// <summary>Runs <c>docker build</c> for the API project (more reliable here than streaming the context via Docker.DotNet).</summary>
    public static async Task BuildApiImageAsync(string imageName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            WorkingDirectory = RepositoryPaths.ApiProjectDirectory,
            RedirectStandardOutput = false,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add("-t");
        startInfo.ArgumentList.Add(imageName);
        startInfo.ArgumentList.Add(".");

        using var process = new Process { StartInfo = startInfo };
        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start docker build.");
        }

        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync().ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"docker build failed with exit code {process.ExitCode}. stderr:{Environment.NewLine}{stderr}");
        }
    }
}
