#region Usings

using System;
using System.Buffers;
using System.Reflection;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.Egress
{
  /// <summary>
  /// An egress message pipeline step that serializes the publishing message.
  /// </summary>
  public class SerializeMessageStep : IStep<MessagePublishingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessagePublishingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var registry = context.Api.MessageTypesRegistry;
      context.Payload = (byte[]) GenericSerialize!
        .MakeGenericMethod(context.Message.GetType())
        .Invoke(this, new[] {context.Message, registry});
      return Task.CompletedTask;
    }

    private byte[] Serialize<TMessage>(TMessage message, IEgressApiMessageTypesRegistry registry) where TMessage : class
    {
      var descriptor = registry.GetDescriptorByMessageType<TMessage>();
      using var memoryOwner = MemoryPool<byte>.Shared.Rent();
      var buffer = memoryOwner.Memory;
      descriptor.Serialize(message, buffer);
      return buffer.ToArray();
    }

    private static readonly MethodInfo GenericSerialize =
      typeof(SerializeMessageStep).GetMethod(nameof(Serialize), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
