#region Usings

using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.IntegrationTests.Tools
{
  [CollectionDefinition(Name)]
  public class EventStoreSetupCollection : ICollectionFixture<EventStoreSetupContainerAsyncFixture>
  {
    public const string Name = nameof(EventStoreSetupCollection);
  }
}
