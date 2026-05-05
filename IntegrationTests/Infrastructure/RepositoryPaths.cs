namespace IntegrationTests.Infrastructure;

internal static class RepositoryPaths
{
    public static string SolutionDirectory { get; } = FindSolutionDirectory();

    public static string ApiProjectDirectory => Path.Combine(SolutionDirectory, "API");

    private static string FindSolutionDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "MyBlog.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate MyBlog.sln (searched upward from the test assembly directory).");
    }
}
