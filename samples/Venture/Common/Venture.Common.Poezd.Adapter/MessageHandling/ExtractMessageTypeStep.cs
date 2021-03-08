#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Extracts message type from message broker headers and puts message type related items into context.
  /// </summary>
  public class ExtractMessageTypeStep : IStep<MessageHandlingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));
      if (context.PublicApi == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.PublicApi));
      if (context.PublicApi.MessageTypesRegistry == null)
        throw context.MakeKeyNotFoundException(nameof(IIngressPublicApi.MessageTypesRegistry));

      var metadata = context.Metadata;

      if (metadata.TryGetValue(VentureApi.Headers.MessageTypeName, out var messageTypeName))
      {
        if (string.IsNullOrWhiteSpace(messageTypeName))
        {
          throw new PoezdOperationException(
            "Message type in its headers is null, an empty or whitespace string. " +
            "By the contract of standard Venture public API it should be specified.");
        }

        context.TypeName = messageTypeName;
      }
      else
      {
        throw new PoezdOperationException(
          "Can not find the message type in its headers. By the contract of standard Venture public API it should be " +
          $"specified in the {VentureApi.Headers.MessageTypeName} Kafka header.");
      }

      var messageTypesRegistry = context.PublicApi.MessageTypesRegistry;

      try
      {
        var messageType = messageTypesRegistry.GetMessageTypeByItsMessageTypeName(messageTypeName);
        context.MessageType = messageType;

        var getDescriptorMethod =
          typeof(IIngressMessageTypesRegistry).GetMethod(nameof(IIngressMessageTypesRegistry.GetDescriptorByMessageTypeName))!
            .MakeGenericMethod(messageType);

        context.Descriptor = getDescriptorMethod.Invoke(messageTypesRegistry, new object?[] {messageTypeName});
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException("Found an unknown message type. Inspect inner exception to find more information.", exception);
      }

      return Task.CompletedTask;
    }
  }
}
