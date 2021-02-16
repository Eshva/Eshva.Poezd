#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Poezd.Adapter;
using Venture.WorkPlanner.Messages;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Extracts message type from message broker headers and sets appropriate metadata in the message handling context.
  /// </summary>
  public class ExtractMessageTypeStep : IStep
  {
    public ExtractMessageTypeStep([NotNull] MessageTypesRegistry messageTypesRegistry)
    {
      _messageTypesRegistry = messageTypesRegistry ?? throw new ArgumentNullException(nameof(messageTypesRegistry));
    }

    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      if (!context.TryTake<Dictionary<string, string>>(ContextKeys.Broker.MessageMetadata, out var metadata))
        return Task.CompletedTask;

      if (metadata.TryGetValue(Api.Headers.MessageTypeName, out var messageTypeName))
      {
        if (string.IsNullOrWhiteSpace(messageTypeName))
        {
          throw new PoezdSkipMessageException(
            "Message type in its headers is null, an empty or whitespace string. " +
            "By the contract of the Work Planner service it should be specified.");
        }

        context.Put(ContextKeys.Application.MessageTypeName, messageTypeName);
      }
      else
      {
        throw new PoezdSkipMessageException(
          "Can not find the message type in its headers. By the contract of the Work Planner service it should be " +
          $"specified in the {ContextKeys.Application.MessageTypeName} Kafka header.");
      }

      try
      {
        context.Put(ContextKeys.Application.MessageType, _messageTypesRegistry.GetType(messageTypeName));
      }
      catch (InvalidOperationException exception)
      {
        throw new PoezdSkipMessageException("Found an unknown message type. Inspect inner exception to find more information.", exception);
      }

      return Task.CompletedTask;
    }

    private readonly MessageTypesRegistry _messageTypesRegistry;
  }
}
