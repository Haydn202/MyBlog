using API.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DotNet.Testcontainers.Networks;
using Testcontainers.MsSql;

namespace IntegrationTests.Infrastructure;

public class SqlServerFixture : IAsyncLifetime
{
    public const string Password = "YourStrong!Passw0rd";
    public const string DatabaseName = "TestDb";
    public const string NetworkAlias = "mssql";

    private readonly INetwork _network;

    private readonly MsSqlContainer _container;

    public SqlServerFixture(INetwork network)
    {
        _network = network;
        _container = new MsSqlBuilder()
            .WithPassword(Password)
            .WithNetwork(_network)
            .WithNetworkAliases(NetworkAlias)
            .Build();
    }

    /// <summary>Connection string for the test host (migrations, direct SQL).</summary>
    public string ConnectionString { get; private set; } = null!;

    /// <summary>Connection string for other containers on the same Docker network.</summary>
    public string DockerNetworkConnectionString { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);

        var masterConnectionString = _container.GetConnectionString();

        await using (var conn = new SqlConnection(masterConnectionString))
        {
            await conn.OpenAsync().ConfigureAwait(false);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        var builder = new SqlConnectionStringBuilder(masterConnectionString)
        {
            InitialCatalog = DatabaseName
        };
        ConnectionString = builder.ConnectionString;

        DockerNetworkConnectionString = new SqlConnectionStringBuilder
        {
            DataSource = $"{NetworkAlias},1433",
            UserID = "sa",
            Password = Password,
            InitialCatalog = DatabaseName,
            TrustServerCertificate = true
        }.ConnectionString;

        await using var context = new DataContext(
            new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(ConnectionString)
                .Options);

        await context.Database.MigrateAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
    }
}
