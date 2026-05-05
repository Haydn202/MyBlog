using IntegrationTests.Builders.User;
using IntegrationTests.Helpers;
using IntegrationTests.Infrastructure;

namespace IntegrationTests.Tests.Users;

[Collection("Integration")]
public class AccountTests(TestFixture fixture)
{
    [Fact]
    public async Task Register_succeeds()
    {
        var api = fixture.CreateApi();
        await api.Accounts
            .Register(
                new RegisterUserRequest()
                    .WithName($"reg_ok_{Guid.NewGuid():N}")
                    .WithEmail($"reg_ok_{Guid.NewGuid():N}@test.local")
                    .WithPassword("password"))
            .Verify();
    }

    [Fact]
    public async Task Register_duplicate_email_returns_bad_request()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"dup_{suffix}@test.local";
        var api = fixture.CreateApi();
        using (var first = await api.Accounts.Register(
                   new RegisterUserRequest()
                       .WithName($"user_a_{suffix}")
                       .WithEmail(email)
                       .WithPassword("password")))
        {
            first.EnsureSuccessStatusCode();
        }

        await api.Accounts
            .Register(
                new RegisterUserRequest()
                    .WithName($"user_b_{suffix}")
                    .WithEmail(email)
                    .WithPassword("password"))
            .Verify();
    }

    [Fact]
    public async Task Login_succeeds()
    {
        var api = fixture.CreateApi();
        await api.Accounts
            .Login(
                new LoginUserRequest()
                    .WithEmail(AuthTestCredentials.AdminEmail)
                    .WithPassword(AuthTestCredentials.AdminPassword))
            .Verify();
    }

    [Fact]
    public async Task Login_invalid_credentials_returns_bad_request()
    {
        var api = fixture.CreateApi();
        await api.Accounts
            .Login(
                new LoginUserRequest()
                    .WithEmail(AuthTestCredentials.AdminEmail)
                    .WithPassword("wrong-password"))
            .Verify();
    }

    [Fact]
    public async Task RefreshToken_without_cookie_returns_no_content()
    {
        var api = fixture.CreateApi();
        await api.Accounts.RefreshToken().Verify();
    }

    [Fact]
    public async Task RefreshToken_after_login_returns_ok()
    {
        var api = fixture.CreateApi(handleCookies: true);
        using (var login = await api.Accounts.Login(
                   new LoginUserRequest()
                       .WithEmail(AuthTestCredentials.AdminEmail)
                       .WithPassword(AuthTestCredentials.AdminPassword)))
        {
            login.EnsureSuccessStatusCode();
        }

        await api.Accounts.RefreshToken().Verify();
    }

    [Fact]
    public async Task Logout_without_authentication_returns_unauthorized()
    {
        var api = fixture.CreateApi();
        await api.Accounts.Logout().Verify();
    }

    [Fact]
    public async Task Logout_with_valid_token_returns_ok()
    {
        var api = fixture.CreateApi();
        var token = await api.Http.LoginGetBearerTokenAsync();
        api.Http.SetBearerToken(token);
        await api.Accounts.Logout().Verify();
    }
}
