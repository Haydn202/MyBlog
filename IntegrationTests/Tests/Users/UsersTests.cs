using System.Text.Json;
using API.Entities;
using IntegrationTests.Builders.User;
using IntegrationTests.Helpers;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Tests.Users;

[Collection("Integration")]
public class UsersTests(TestFixture fixture)
{
    /// <summary>Fixed id for route-only tests so Verify's inline GUID scrubbing keeps snapshots stable.</summary>
    private static readonly Guid UsersRoutePlaceholderId =
        Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    [Fact]
    public async Task GetUsers_without_token_returns_unauthorized()
    {
        var api = fixture.CreateApi();
        await api.Users.GetUsers().Verify();
    }

    [Fact]
    public async Task GetUsers_as_non_admin_returns_forbidden()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"member_{suffix}@test.local";
        var registerApi = fixture.CreateApi();
        using (var reg = await registerApi.Accounts.Register(
                   new RegisterUserRequest()
                       .WithName($"member_{suffix}")
                       .WithEmail(email)
                       .WithPassword("password")))
        {
            reg.EnsureSuccessStatusCode();
        }

        var userApi = fixture.CreateApi();
        var token = await userApi.Http.LoginGetBearerTokenAsync(email, "password");
        userApi.Http.SetBearerToken(token);
        await userApi.Users.GetUsers().Verify();
    }

    [Fact]
    public async Task GetUsers_as_admin_returns_ok()
    {
        var api = fixture.CreateApi();
        var token = await api.Http.LoginGetBearerTokenAsync();
        api.Http.SetBearerToken(token);
        await api.Users.GetUsers().Verify();
    }

    [Fact]
    public async Task GetUser_without_token_returns_unauthorized()
    {
        var api = fixture.CreateApi();
        await api.Users.GetUser(UsersRoutePlaceholderId).Verify();
    }

    [Fact]
    public async Task GetUser_as_non_admin_returns_forbidden()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"get_one_forbidden_{suffix}@test.local";
        var registerApi = fixture.CreateApi();
        using (var reg = await registerApi.Accounts.Register(
                   new RegisterUserRequest()
                       .WithName($"get_one_forbidden_{suffix}")
                       .WithEmail(email)
                       .WithPassword("password")))
        {
            reg.EnsureSuccessStatusCode();
        }

        var userApi = fixture.CreateApi();
        var token = await userApi.Http.LoginGetBearerTokenAsync(email, "password");
        userApi.Http.SetBearerToken(token);
        await userApi.Users.GetUser(UsersRoutePlaceholderId).Verify();
    }

    [Fact]
    public async Task GetUser_unknown_id_returns_not_found()
    {
        var api = fixture.CreateApi();
        var token = await api.Http.LoginGetBearerTokenAsync();
        api.Http.SetBearerToken(token);
        await api.Users.GetUser(Guid.NewGuid()).VerifyResponseMetaOnly();
    }

    [Fact]
    public async Task GetUser_existing_returns_ok()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"get_one_{suffix}@test.local";
        var regApi = fixture.CreateApi();
        using var reg = await regApi.Accounts.Register(
            new RegisterUserRequest()
                .WithName($"get_one_{suffix}")
                .WithEmail(email)
                .WithPassword("password"));
        reg.EnsureSuccessStatusCode();
        using var doc = await JsonDocument.ParseAsync(await reg.Content.ReadAsStreamAsync());
        var userId = Guid.Parse(doc.RootElement.GetProperty("id").GetString()!);

        var api = fixture.CreateApi();
        var token = await api.Http.LoginGetBearerTokenAsync();
        api.Http.SetBearerToken(token);
        await api.Users.GetUser(userId).VerifyResponseMetaOnly();
    }

    [Fact]
    public async Task UpdateUserRole_without_token_returns_unauthorized()
    {
        var api = fixture.CreateApi();
        await api.Users
            .UpdateRole(UsersRoutePlaceholderId, new UpdateRoleRequest().WithRole(nameof(Role.Contributor)))
            .Verify();
    }

    [Fact]
    public async Task UpdateUserRole_as_non_admin_returns_forbidden()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"patch_forbidden_{suffix}@test.local";
        var registerApi = fixture.CreateApi();
        using (var reg = await registerApi.Accounts.Register(
                   new RegisterUserRequest()
                       .WithName($"patch_forbidden_{suffix}")
                       .WithEmail(email)
                       .WithPassword("password")))
        {
            reg.EnsureSuccessStatusCode();
        }

        var userApi = fixture.CreateApi();
        var token = await userApi.Http.LoginGetBearerTokenAsync(email, "password");
        userApi.Http.SetBearerToken(token);
        await userApi.Users
            .UpdateRole(UsersRoutePlaceholderId, new UpdateRoleRequest().WithRole(nameof(Role.Contributor)))
            .Verify();
    }

    [Fact]
    public async Task UpdateUserRole_unknown_user_returns_bad_request()
    {
        var api = fixture.CreateApi();
        var token = await api.Http.LoginGetBearerTokenAsync();
        api.Http.SetBearerToken(token);
        await api.Users
            .UpdateRole(UsersRoutePlaceholderId, new UpdateRoleRequest().WithRole(nameof(Role.Contributor)))
            .VerifyResponseOnly();
    }

    [Fact]
    public async Task UpdateUserRole_invalid_role_returns_bad_request()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"patch_bad_{suffix}@test.local";
        var regApi = fixture.CreateApi();
        using var regBad = await regApi.Accounts.Register(
            new RegisterUserRequest()
                .WithName($"patch_bad_{suffix}")
                .WithEmail(email)
                .WithPassword("password"));
        regBad.EnsureSuccessStatusCode();
        using var docBad = await JsonDocument.ParseAsync(await regBad.Content.ReadAsStreamAsync());
        var userIdBad = Guid.Parse(docBad.RootElement.GetProperty("id").GetString()!);

        var apiBad = fixture.CreateApi();
        var tokenBad = await apiBad.Http.LoginGetBearerTokenAsync();
        apiBad.Http.SetBearerToken(tokenBad);
        await apiBad.Users
            .UpdateRole(userIdBad, new UpdateRoleRequest().WithRole("NotARealRole"))
            .VerifyResponseOnly();
    }

    [Fact]
    public async Task UpdateUserRole_valid_role_returns_ok()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"patch_ok_{suffix}@test.local";
        var regApi = fixture.CreateApi();
        using var regOk = await regApi.Accounts.Register(
            new RegisterUserRequest()
                .WithName($"patch_ok_{suffix}")
                .WithEmail(email)
                .WithPassword("password"));
        regOk.EnsureSuccessStatusCode();
        using var docOk = await JsonDocument.ParseAsync(await regOk.Content.ReadAsStreamAsync());
        var userIdOk = Guid.Parse(docOk.RootElement.GetProperty("id").GetString()!);

        var apiOk = fixture.CreateApi();
        var tokenOk = await apiOk.Http.LoginGetBearerTokenAsync();
        apiOk.Http.SetBearerToken(tokenOk);
        await apiOk.Users
            .UpdateRole(userIdOk, new UpdateRoleRequest().WithRole(nameof(Role.Contributor)))
            .VerifyResponseMetaOnly();
    }
}
