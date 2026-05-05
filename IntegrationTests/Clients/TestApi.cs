namespace IntegrationTests.Clients;

public class TestApi(HttpClient client)
{
    public HttpClient Http { get; } = client;

    public AccountsClient Accounts { get; } = new(client);
    public UsersClient Users { get; } = new(client);
}
