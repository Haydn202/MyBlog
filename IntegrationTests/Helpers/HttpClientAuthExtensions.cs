using System.Net.Http.Headers;
using System.Text.Json;
using IntegrationTests.Builders.User;
using IntegrationTests.Clients;

namespace IntegrationTests.Helpers;

public static class AuthTestCredentials
{
    public const string AdminEmail = "admin@email.com";
    public const string AdminPassword = "admin";
}

public static class HttpClientAuthExtensions
{
    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void ClearBearerToken(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }

    public static Task<string> LoginGetBearerTokenAsync(this HttpClient client) =>
        client.LoginGetBearerTokenAsync(AuthTestCredentials.AdminEmail, AuthTestCredentials.AdminPassword);

    public static async Task<string> LoginGetBearerTokenAsync(this HttpClient client, string email, string password)
    {
        var api = new TestApi(client);
        using var response = await api.Accounts.Login(
            new LoginUserRequest()
                .WithEmail(email)
                .WithPassword(password));
        response.EnsureSuccessStatusCode();
        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var token = doc.RootElement.GetProperty("token").GetString();
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Login response did not contain a token.");
        }

        return token;
    }
}
