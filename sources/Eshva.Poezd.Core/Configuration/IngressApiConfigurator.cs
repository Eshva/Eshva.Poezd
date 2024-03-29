#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// An ingress API configurator.
  /// </summary>
  public class IngressApiConfigurator
  {
    /// <summary>
    /// Constructs a new instance of an ingress API configurator.
    /// </summary>
    /// <param name="configuration">
    /// The ingress API configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The ingress API configuration is not specified.
    /// </exception>
    public IngressApiConfigurator([NotNull] IngressApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the ID of this message ingress API.
    /// </summary>
    /// <param name="id">
    /// The ingress API ID.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The ID is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    public IngressApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
      if (_configuration.Id != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "ID",
          "broker ingress API",
          nameof(WithId));
      }

      _configuration.Id = id;
      return this;
    }

    /// <summary>
    /// Sets the type of pipe fitter that sets up the pipeline used to handle received published messages.
    /// </summary>
    /// <typeparam name="TPipeFitter">
    /// The type of pipe fitter.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithPipeFitter<TPipeFitter>() where TPipeFitter : IPipeFitter
    {
      if (_configuration.PipeFitterType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "pipe fitter type",
          "broker ingress API",
          nameof(WithPipeFitter));
      }

      _configuration.PipeFitterType = typeof(TPipeFitter);
      return this;
    }

    /// <summary>
    /// Sets the type of message key.
    /// </summary>
    /// <remarks>
    /// The key required only for some message broker. For instance Kafka uses it to select partition of a topic to place a
    /// message to. This type used to select proper deserializer for the message key.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithMessageKey<TMessageKey>()
    {
      if (_configuration.MessageKeyType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message key type",
          "broker ingress API",
          nameof(WithMessageKey));
      }

      _configuration.MessageKeyType = typeof(TMessageKey);
      return this;
    }

    /// <summary>
    /// Sets the type of message payload.
    /// </summary>
    /// <remarks>
    /// This type used to select proper deserializer for the message payload.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithMessagePayload<TMessagePayload>()
    {
      if (_configuration.MessagePayloadType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message payload type",
          "broker ingress API",
          nameof(WithMessagePayload));
      }

      _configuration.MessagePayloadType = typeof(TMessagePayload);
      return this;
    }

    /// <summary>
    /// Sets the type of message types registry.
    /// </summary>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
      where TMessageTypesRegistry : IIngressApiMessageTypesRegistry
    {
      if (_configuration.MessageTypesRegistryType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message types registry type",
          "broker ingress API",
          nameof(WithMessageTypesRegistry));
      }

      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    /// <summary>
    /// Sets the type of queue name patterns provider.
    /// </summary>
    /// <remarks>
    /// This provider used to get queue names the API messages are published to. The format of these patterns depends on the
    /// message broker. For instance Kafka supports Regex patterns.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      if (_configuration.QueueNamePatternsProviderType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "queue name patterns provider type",
          "broker ingress API",
          nameof(WithQueueNamePatternsProvider));
      }

      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    /// <summary>
    /// Sets the type of the message handlers registry.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IHandlerRegistry" />.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public IngressApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      if (_configuration.HandlerRegistryType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "handler registry type",
          "broker ingress API",
          nameof(WithHandlerRegistry));
      }

      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    private readonly IngressApiConfiguration _configuration;
  }
}
