using System.Net.Http.Json;
using IntegrationTests.Builders.User;

namespace IntegrationTests.Clients;

public class AccountsClient(HttpClient client)
{
    private readonly HttpClient _client = client;

    public Task<HttpResponseMessage> Register(RegisterUserRequest request) =>
        _client.PostAsJsonAsync("Accounts/register", request.Build());

    public Task<HttpResponseMessage> Login(LoginUserRequest request) =>
        _client.PostAsJsonAsync("Accounts/login", request.Build());

    public Task<HttpResponseMessage> RefreshToken() =>
        _client.PostAsync("Accounts/refresh-token", null);

    public Task<HttpResponseMessage> Logout() =>
        _client.PostAsync("Accounts/logout", null);
}
