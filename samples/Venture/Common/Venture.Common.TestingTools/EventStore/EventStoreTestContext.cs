#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.TestingTools.EventStore
{
  public class EventStoreTestContext
  {
    public EventStoreTestContext([NotNull] EventStoreClient client)
    {
      _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public Task<IWriteResult> AppendToStream(
      EventData @event,
      string streamName,
      CancellationToken cancellationToken) =>
      _client.AppendToStreamAsync(
        streamName,
        StreamState.NoStream,
        new[] {@event},
        cancellationToken: cancellationToken);

    private readonly EventStoreClient _client;
  }
}
