using System.Net;
using System.Net.Http;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using IntegrationTests.Clients;

namespace IntegrationTests.Infrastructure;

public class TestFixture : IAsyncLifetime
{
    private const ushort ApiContainerPort = 8080;
    private const string ApiImageName = "myblog-api:integration-test";

    private INetwork _network = null!;
    private SqlServerFixture _dbFixture = null!;
    private IContainer _apiContainer = null!;
    private Uri _apiBaseUri = null!;

    public async ValueTask InitializeAsync()
    {
        _network = new NetworkBuilder().Build();
        await _network.CreateAsync().ConfigureAwait(false);

        _dbFixture = new SqlServerFixture(_network);
        await _dbFixture.InitializeAsync().ConfigureAwait(false);

        await DockerCli.BuildApiImageAsync(ApiImageName).ConfigureAwait(false);

        _apiContainer = new ContainerBuilder()
            .WithImage(ApiImageName)
            .WithNetwork(_network)
            .WithPortBinding(ApiContainerPort, true)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Testing")
            .WithEnvironment("ASPNETCORE_URLS", $"http://+:{ApiContainerPort}")
            .WithEnvironment("ConnectionStrings__DefaultConnection", _dbFixture.DockerNetworkConnectionString)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r.ForPort(ApiContainerPort).ForPath("/")))
            .Build();

        await _apiContainer.StartAsync().ConfigureAwait(false);

        var hostPort = _apiContainer.GetMappedPublicPort(ApiContainerPort);
        _apiBaseUri = new UriBuilder(Uri.UriSchemeHttp, IPAddress.Loopback.ToString(), hostPort).Uri;
    }

    public TestApi CreateApi(bool handleCookies = false)
    {
        var handler = new HttpClientHandler();
        if (handleCookies)
        {
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();
        }

        var client = new HttpClient(handler) { BaseAddress = _apiBaseUri };
        return new TestApi(client);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_apiContainer is not null)
            {
                await _apiContainer.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            try
            {
                if (_dbFixture is not null)
                {
                    await _dbFixture.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                if (_network is not null)
                {
                    await _network.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
