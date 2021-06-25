#region Usings

using System.Threading.Tasks;
using Eshva.Common.Testing;
using EventStore.Client;
using JetBrains.Annotations;
using Venture.Common.TestingTools.EventStore;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  public class EventStoreSetupContainerAsyncFixture : IAsyncLifetime
  {
    public EventStoreSetupContainerAsyncFixture()
    {
      ContainerName = $"test-eventstore-{Randomize.String(length: 10)}";
      ExposedHttpPort = NetworkTools.GetFreeTcpPort(rangeStart: 52000);
      _container = new EventStoreDockerContainer(
        new EventStoreDockerContainerConfiguration
        {
          ContainerName = ContainerName,
          ExposedHttpPort = ExposedHttpPort
        });
    }

    public string ContainerName { get; }

    public string HostName { get; set; } = "localhost";

    public ushort ExposedHttpPort { get; }

    public bool IsSecure { get; set; }

    public EventStoreClient Client { get; private set; }

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
      await _container.StartAsync();
      RecreateClient();
    }

    public async Task DisposeAsync()
    {
      if (Client != null) await Client.DisposeAsync();
      await _container.DisposeAsync();
    }

    private void RecreateClient()
    {
      Client?.Dispose();
      ConnectionString = @$"esdb://{HostName}:{ExposedHttpPort}?tls={IsSecure}";
      var settings = EventStoreClientSettings.Create(ConnectionString);
      settings.ConnectionName = ContainerName;
      // settings.DefaultCredentials = new UserCredentials(@"admin", @"changeit");
      Client = new EventStoreClient(settings);
    }

    private readonly EventStoreDockerContainer _container;
  }
}
