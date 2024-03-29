#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The message broker ingress.
  /// </summary>
  internal class BrokerIngress : IBrokerIngress
  {
    /// <summary>
    /// Construct a new instance of message broker.
    /// </summary>
    /// <param name="messageBroker">
    /// The message broker this ingress belongs to.
    /// </param>
    /// <param name="configuration">
    /// The message broker configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// Can not get a required service from <paramref name="serviceProvider" />.
    /// </exception>
    public BrokerIngress(
      [NotNull] IMessageBroker messageBroker,
      [NotNull] BrokerIngressConfiguration configuration,
      [NotNull] IDiContainerAdapter serviceProvider)
    {
      _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      Driver = configuration.Driver ?? throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.Driver)}");
      Apis = configuration.Apis.Select(api => new IngressApi(api, serviceProvider)).ToList().AsReadOnly();
      _queueNameMatcher = (IQueueNameMatcher) serviceProvider.GetService(configuration.QueueNameMatcherType);
      EnterPipeFitter = GetEnterPipeFitter();
      ExitPipeFitter = GetExitPipeFitter();
    }

    /// <inheritdoc />
    public BrokerIngressConfiguration Configuration { get; }

    /// <inheritdoc />
    public IBrokerIngressDriver Driver { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<IIngressApi> Apis { get; }

    /// <inheritdoc />
    public IPipeFitter EnterPipeFitter { get; }

    /// <inheritdoc />
    public IPipeFitter ExitPipeFitter { get; }

    /// <inheritdoc />
    public Task RouteIngressMessage(
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata) =>
      _messageBroker.RouteIngressMessage(
        queueName,
        receivedOnUtc,
        key,
        payload,
        metadata);

    /// <inheritdoc />
    public IIngressApi GetApiByQueueName(string queueName)
    {
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
      var ingressApi = Apis
        .Where(api => api.GetQueueNamePatterns().Any(queueNamePattern => _queueNameMatcher.DoesMatch(queueName, queueNamePattern)))
        .ToArray();

      // TODO: May be I should specify error code in PoezdOperationException to distinct errors.
      if (!ingressApi.Any()) throw new PoezdOperationException($"Message queue '{queueName}' doesn't belong to any API.");
      if (ingressApi.Length > 1)
      {
        throw new PoezdOperationException(
          $"Message queue '{queueName}' belongs to a few APIs: {string.Join(", ", ingressApi.Select(api => $"'{api.Id}'"))}.");
      }

      return ingressApi.Single();
    }

    /// <inheritdoc />
    public void Initialize()
    {
      if (_isInitialized) throw new PoezdOperationException($"Broker '{_messageBroker.Id}' ingress already initialized.");

      Driver.Initialize(
        this,
        Apis,
        _serviceProvider);
      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));
      if (!_isInitialized)
      {
        throw new PoezdOperationException(
          $"Broker '{_messageBroker.Id}' ingress is not initialized yet. " +
          "You should call Initialize() before calling StartConsumeMessages().");
      }

      return Driver.StartConsumeMessages(queueNamePatterns, cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose() => Driver.Dispose();

    private IPipeFitter GetEnterPipeFitter() =>
      _serviceProvider.GetService<IPipeFitter>(
        Configuration.EnterPipeFitterType,
        exception => new PoezdConfigurationException(
          "Can not get instance of the message broker ingress enter pipe fitter of type " +
          $"'{Configuration.EnterPipeFitterType.FullName}'. You should register this type in DI-container.",
          exception));

    private IPipeFitter GetExitPipeFitter() =>
      _serviceProvider.GetService<IPipeFitter>(
        Configuration.ExitPipeFitterType,
        exception => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress exit pipe fitter of type '{Configuration.ExitPipeFitterType.FullName}'. " +
          "You should register this type in DI-container.",
          exception));

    private readonly IMessageBroker _messageBroker;
    private readonly IQueueNameMatcher _queueNameMatcher;
    private readonly IDiContainerAdapter _serviceProvider;
    private bool _isInitialized;
  }
}
