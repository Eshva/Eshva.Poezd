#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  /// <summary>
  /// An egress message pipeline step that gets queue names to which the message should be published to.
  /// </summary>
  public class GetTopicNameStep : IStep<MessagePublishingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessagePublishingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var getQueueNames = GenericGetQueueName!.MakeGenericMethod(context.Message.GetType());
      context.QueueNames = (IReadOnlyCollection<string>) getQueueNames.Invoke(
        this,
        new object?[] {context.PublicApi.MessageTypesRegistry});
      return Task.CompletedTask;
    }

    private IReadOnlyCollection<string> GetQueueNames<TMessage>(IMessageTypesRegistry registry) where TMessage : class =>
      registry.GetDescriptorByMessageType<TMessage>().QueueNames;

    private static readonly MethodInfo GenericGetQueueName =
      typeof(GetTopicNameStep).GetMethod(nameof(GetQueueNames), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
