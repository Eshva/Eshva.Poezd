#region Usings

using System.Threading.Tasks;
using Eshva.Common.Testing;
using EventStore.Client;
using JetBrains.Annotations;
using Venture.Common.TestingTools.EventStore;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.IntegrationTests.Tools
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

    public string ConnectionString { get; private set; }

    public EventStoreClient Client { get; private set; }

    public EventStorePersistentSubscriptionsClient PersistentSubscriptionsClient { get; private set; }

    public string EntityName { get; private set; }

    public EventStoreProjectionManagementClient ProjectionManagementClient { get; private set; }

    public string SubscriptionGroupName { get; private set; }

    public string ByCategoryStreamName { get; private set; }

    public async Task InitializeAsync()
    {
      await _container.StartAsync();
      BuildConnectionString();
      RecreateClient();
      RecreatePersistentSubscriptionsClient();
      RecreateProjectionManagementClient();

      GenerateEntityName();
      GenerateByCategoryStreamName();
      GenerateSubscriptionGroupName();
      await CreatePersistentSubscriptions();
    }

    public async Task DisposeAsync()
    {
      if (Client != null) await Client.DisposeAsync();
      if (PersistentSubscriptionsClient != null) await PersistentSubscriptionsClient.DisposeAsync();
      if (ProjectionManagementClient != null) await ProjectionManagementClient.DisposeAsync();

      await _container.DisposeAsync();
    }

    private void RecreateProjectionManagementClient()
    {
      ProjectionManagementClient?.Dispose();
      ProjectionManagementClient = new EventStoreProjectionManagementClient(EventStoreClientSettings.Create(ConnectionString));
    }

    private Task CreatePersistentSubscriptions() =>
      PersistentSubscriptionsClient.CreateAsync(
        ByCategoryStreamName,
        SubscriptionGroupName,
        new PersistentSubscriptionSettings(
          startFrom: StreamPosition.Start,
          resolveLinkTos: true,
          namedConsumerStrategy: SystemConsumerStrategies.Pinned));

    private void GenerateSubscriptionGroupName() => SubscriptionGroupName = $"group-{Randomize.String(length: 8)}";

    private void GenerateEntityName() => EntityName = $"entity";
    // private void GenerateEntityName() => EntityName = $"entity{Randomize.String(length: 10)}";

    private void GenerateByCategoryStreamName() => ByCategoryStreamName = $"$ce-{EntityName}";

    private void BuildConnectionString() => ConnectionString = @$"esdb://{HostName}:{ExposedHttpPort}?tls={IsSecure}";

    private void RecreateClient()
    {
      Client?.Dispose();
      Client = new EventStoreClient(EventStoreClientSettings.Create(ConnectionString));
    }

    private void RecreatePersistentSubscriptionsClient()
    {
      PersistentSubscriptionsClient?.Dispose();
      var settings = EventStoreClientSettings.Create(ConnectionString);
      settings.ConnectionName = ContainerName;
      // settings.DefaultCredentials = new UserCredentials(@"admin", @"changeit");

      PersistentSubscriptionsClient = new EventStorePersistentSubscriptionsClient(settings);
    }

    private readonly EventStoreDockerContainer _container;
  }
}
