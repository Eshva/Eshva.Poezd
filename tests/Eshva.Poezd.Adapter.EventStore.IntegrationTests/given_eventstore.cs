#region Usings

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Common.Tpl;
using Eshva.Poezd.Adapter.EventStore.Ingress;
using Eshva.Poezd.Adapter.EventStore.IntegrationTests.Tools;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Routing;
using EventStore.Client;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.IntegrationTests
{
  [Collection(EventStoreSetupCollection.Name)]
  public class given_eventstore
  {
    public given_eventstore(EventStoreSetupContainerAsyncFixture fixture, ITestOutputHelper testOutput)
    {
      _fixture = fixture;
      _testOutput = testOutput;
    }

    [Fact]
    public async Task when_testing_persistent_subscriptions_it_should_just_work()
    {
      var source = new CancellationTokenSource(TimeSpan.FromMinutes(value: 5));
      var doneOrTimeout = source.Token;

      ResolvedEvent messageReceived = default;
      var res1 = await _fixture.PersistentSubscriptionsClient.SubscribeAsync(
        _fixture.ByCategoryStreamName,
        _fixture.SubscriptionGroupName,
        (
          subscription,
          resolvedEvent,
          retryCount,
          cancellationToken) =>
        {
          _testOutput.WriteLine(
            $"Message appeared from {_fixture.ByCategoryStreamName} with '{Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span)}' as payload.");
          _testOutput.WriteLine($"Some integer is '{retryCount}'.");

          messageReceived = resolvedEvent;
          source.Cancel();
          return Task.CompletedTask;
        },
        SubscriptionDropped,
        cancellationToken: doneOrTimeout);
      await PublishMessage(doneOrTimeout);

      // source.Cancel();
      await doneOrTimeout;

      messageReceived.Event.Should().NotBeNull();
    }

    [Fact]
    public async Task when_testing_persistent_subscriptions_it_should_just_work1()
    {
      var currentCancellationSource = new CancellationTokenSource(TimeSpan.FromMinutes(value: 5));
      var doneOrTimeout = currentCancellationSource.Token;

      // await CreateSubscriptionGroups(doneOrTimeout);
      ResolvedEvent messageReceived = default;
      await _fixture.Client.SubscribeToStreamAsync(
        $"{_fixture.EntityName}-{_entityId:N}",
        (
          subscription,
          resolvedEvent,
          cancellationToken) =>
        {
          _testOutput.WriteLine(
            $"Message appeared from {_fixture.ByCategoryStreamName} with '{Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span)}' as payload.");
          messageReceived = resolvedEvent;
          currentCancellationSource.Cancel();
          return Task.CompletedTask;
        },
        resolveLinkTos: false,
        cancellationToken: doneOrTimeout);
      await PublishMessage1(doneOrTimeout);

      // source.Cancel();
      await doneOrTimeout;

      messageReceived.Event.Should().NotBeNull();
    }

    [Fact]
    public async Task when_setup_connection_it_should_handle_messages_from_eventstore()
    {
      var doneOrTimeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));

      var isMessageHandled = false;
      var router = CreateConfiguredMessageRouter(() => isMessageHandled = true);
      // await router.Start(doneOrTimeout);

      await PublishMessage(doneOrTimeout);
      await doneOrTimeout;

      isMessageHandled.Should().BeTrue("published message should be handled be properly configured message router");
    }

    private Task<IWriteResult> PublishMessage1(CancellationToken doneOrTimeout)
    {
      return _fixture.Client.AppendToStreamAsync(
        $"{_fixture.EntityName}-{_entityId:N}",
        StreamState.Any,
        new[]
        {
          new EventData(
            Uuid.NewUuid(),
            "Created",
            Encoding.UTF8.GetBytes("payload"),
            Encoding.UTF8.GetBytes("metadata"))
        },
        cancellationToken: doneOrTimeout);
    }

    private void SubscriptionDropped(
      PersistentSubscription subscription,
      SubscriptionDroppedReason reason,
      Exception? exception)
    {
      _testOutput.WriteLine($"Subscription to '{_fixture.ByCategoryStreamName}' was dropped with reason: '{reason:G}'.");
    }

    /*
    private Task CreateSubscriptionGroups(CancellationToken doneOrTimeout)
    {
      var settings = new PersistentSubscriptionSettings(startFrom: StreamPosition.Start, resolveLinkTos: true);
      return _fixture.PersistentSubscriptionsClient.CreateAsync(
        _fixture.ByCategoryStreamName,
        _fixture.SubscriptionGroupName,
        settings,
        cancellationToken: doneOrTimeout);
    }
    */

    private Task<IWriteResult> PublishMessage(CancellationToken doneOrTimeout)
    {
      return _fixture.Client.AppendToStreamAsync(
        $"{_fixture.EntityName}-{Guid.NewGuid():N}",
        StreamState.Any,
        new[]
        {
          new EventData(
            Uuid.NewUuid(),
            "Created",
            Encoding.UTF8.GetBytes("payload"),
            Encoding.UTF8.GetBytes("metadata"))
        },
        cancellationToken: doneOrTimeout);
    }

    private IMessageRouter CreateConfiguredMessageRouter(Action messageHandledAction)
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterSingleton<Utf8ByteStringHeaderValueCodec>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<TestQueueNamePatternsProvider>();
      container.RegisterSingleton<TestIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<TestServiceHandlersRegistry>();
      container.RegisterSingleton<TestIngressEnterPipeline>();
      container.RegisterSingleton<TestIngressExitPipeline>();

      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("test-eventstoredb-server")
                .Ingress(
                  ingress => ingress
                    .WithEventStoreDriver(
                      driver => driver
                        .WithConnection(_fixture.ConnectionString)
                        .WithHeaderValueCodec<Utf8ByteStringHeaderValueCodec>())
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    // .WithEnterPipeFitter<TestIngressEnterPipeline>()
                    // .WithExitPipeFitter<TestIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("ingress-case-office")
                        .WithQueueNamePatternsProvider<TestQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<TestIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<TestServiceHandlersRegistry>()
                    ))
                .WithoutEgress()));

      // TODO: Call from an exit pipeline step.
      messageHandledAction();

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container.GetInstance<IMessageRouter>();
    }

    private readonly Guid _entityId = Guid.NewGuid();

    private readonly EventStoreSetupContainerAsyncFixture _fixture;
    private readonly ITestOutputHelper _testOutput;
  }
}
