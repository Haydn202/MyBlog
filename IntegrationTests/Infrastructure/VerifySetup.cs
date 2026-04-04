using System.Globalization;
using System.Runtime.CompilerServices;
using VerifyXunit;

namespace IntegrationTests.Infrastructure;

public class VerifySetup
{
    [ModuleInitializer]
    public static void Init()
    {
        // PathInfo prefix is combined with the method name → `{type.Name}.{method.Name}.verified.txt`
        Verifier.DerivePathInfo((_, projectDirectory, type, method) =>
            new(Path.Combine(projectDirectory, "Snapshots", type.Name), type.Name));

        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.ScrubInlineDateTimes(string.Empty, CultureInfo.InvariantCulture, ScrubberLocation.First);
        VerifierSettings.SortPropertiesAlphabetically();
        VerifierSettings.IgnoreMember("id");
        VerifierSettings.IgnoreMember("createdAt");
        VerifierSettings.IgnoreMember("token");
        VerifierSettings.IgnoreMember("refreshToken");
        VerifierSettings.IgnoreMember("passwordHash");
        VerifierSettings.IgnoreMember("securityStamp");
        VerifierSettings.IgnoreMember("concurrencyStamp");
        VerifierSettings.IgnoreMember("phoneNumber");
        VerifierSettings.IgnoreMember("refreshTokenExpires");
        VerifierSettings.IgnoreMember("correlationId");

        VerifierSettings.ScrubLinesWithReplace(line =>
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                return line.Replace(trimmed, "Bearer <scrubbed>", StringComparison.Ordinal);
            }

            if (line.Contains("refreshToken=", StringComparison.OrdinalIgnoreCase))
            {
                return "        refreshToken=Scrubbed; expires=Scrubbed; path=/; secure; samesite=strict; httponly";
            }

            return line;
        }, ScrubberLocation.First);
    }
}
