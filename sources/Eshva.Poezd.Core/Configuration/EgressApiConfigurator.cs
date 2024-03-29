#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// An egress API configurator.
  /// </summary>
  public class EgressApiConfigurator
  {
    /// <summary>
    /// Constructs a new instance of an egress API configurator.
    /// </summary>
    /// <param name="configuration">
    /// The egress API configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The egress API configuration is not specified.
    /// </exception>
    public EgressApiConfigurator([NotNull] EgressApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the ID of this message egress API.
    /// </summary>
    /// <param name="id">
    /// The egress API ID.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The ID is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    public EgressApiConfigurator WithId(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
      if (_configuration.Id != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "ID",
          "broker egress API",
          nameof(WithId));
      }

      _configuration.Id = id;
      return this;
    }

    /// <summary>
    /// Sets the type of pipe fitter that sets up the pipeline used to prepare published messages.
    /// </summary>
    /// <typeparam name="TPipeFitter">
    /// The type of pipe fitter.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public EgressApiConfigurator WithPipeFitter<TPipeFitter>() where TPipeFitter : IPipeFitter
    {
      if (_configuration.PipeFitterType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "pipe fitter",
          "broker egress API",
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
    /// message to. This type used to select proper serializer for the message key.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public EgressApiConfigurator WithMessageKey<TMessageKey>()
    {
      if (_configuration.MessageKeyType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message key type",
          "broker egress API",
          nameof(WithMessageKey));
      }

      _configuration.MessageKeyType = typeof(TMessageKey);
      return this;
    }

    /// <summary>
    /// Sets the type of message payload.
    /// </summary>
    /// <remarks>
    /// This type used to select proper serializer for the message payload.
    /// </remarks>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public EgressApiConfigurator WithMessagePayload<TMessagePayload>()
    {
      if (_configuration.MessagePayloadType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message payload type",
          "broker egress API",
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
    public EgressApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
      where TMessageTypesRegistry : IEgressApiMessageTypesRegistry
    {
      if (_configuration.MessageTypesRegistryType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "message types registry type",
          "broker egress API",
          nameof(WithMessageTypesRegistry));
      }

      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly EgressApiConfiguration _configuration;
  }
}
