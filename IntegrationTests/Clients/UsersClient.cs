using System.Net.Http.Json;
using IntegrationTests.Builders.User;

namespace IntegrationTests.Clients;

public class UsersClient(HttpClient client)
{
    private readonly HttpClient _client = client;

    public Task<HttpResponseMessage> GetUsers() =>
        _client.GetAsync("Users");

    public Task<HttpResponseMessage> GetUser(Guid id) =>
        _client.GetAsync($"Users/{id}");

    public Task<HttpResponseMessage> UpdateRole(Guid id, UpdateRoleRequest request) =>
        _client.PatchAsJsonAsync($"Users/{id}", request.Build());
}
