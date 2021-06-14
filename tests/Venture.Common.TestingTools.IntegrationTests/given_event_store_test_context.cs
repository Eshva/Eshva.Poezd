#region Usings

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eshva.Common.Testing;
using Eshva.Common.Tpl;
using EventStore.Client;
using FluentAssertions;
using Venture.Common.TestingTools.EventStore;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [Collection(EventStoreSetupCollection.Name)]
  public class given_event_store_test_context
  {
    public given_event_store_test_context(EventStoreSetupContainerAsyncFixture fixture)
    {
      _fixture = fixture;
    }

    [Fact]
    public async Task when_append_to_stream_it_should_append_event_to_specified_stream()
    {
      var timeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));
      var testContext = new EventStoreTestContext(_fixture.Client);

      const string eventPayload = "event payload";
      const string eventType = "Create";
      const string eventMetadata = "event metadata";
      var @event = new EventData(
        Uuid.NewUuid(),
        eventType,
        Encoding.UTF8.GetBytes(eventPayload),
        Encoding.UTF8.GetBytes(eventMetadata));
      var streamName = Randomize.String(length: 10);

      await testContext.AppendToStream(
        @event,
        streamName,
        timeout);

      var result = _fixture.Client.ReadStreamAsync(
        Direction.Forwards,
        streamName,
        StreamPosition.Start);
      var events = await result.ToListAsync(timeout);
      events.Should().HaveCount(expected: 1, "a single event has been appended");
      var eventRecord = events.Single().Event;
      Encoding.UTF8.GetString(eventRecord.Data.Span).Should().Be(eventPayload);
      Encoding.UTF8.GetString(eventRecord.Metadata.Span).Should().Be(eventMetadata);
      eventRecord.EventType.Should().Be(eventType);
    }

    private readonly EventStoreSetupContainerAsyncFixture _fixture;
  }
}
