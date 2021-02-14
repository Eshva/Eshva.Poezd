#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public readonly struct HandlerDescriptor
  {
    private static readonly Func<object, VentureContext, Task> Nope = (message, context) => Task.CompletedTask;

    public HandlerDescriptor(
      [NotNull] Type type,
      [NotNull] Type messageType,
      [NotNull] Func<object, VentureContext, Task> onHandle)
    {
      HandlerType = type ?? throw new ArgumentNullException(nameof(type));
      MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
      OnHandle = onHandle ?? throw new ArgumentNullException(nameof(onHandle));
    }

    public Type HandlerType { get; }

    public Type MessageType { get; }

    public Func<object, VentureContext, Task> OnHandle { get; }
  }
}
