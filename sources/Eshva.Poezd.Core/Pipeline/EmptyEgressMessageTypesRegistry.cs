#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  internal class EmptyEgressMessageTypesRegistry : IEgressMessageTypesRegistry
  {
    public string GetMessageTypeNameByItsMessageType(Type messageType) =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    public IEgressMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    public bool DoesOwn<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");
  }
}
