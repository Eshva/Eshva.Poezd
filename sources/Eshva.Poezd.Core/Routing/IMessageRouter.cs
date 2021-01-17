#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageRouter
  {
    Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] brokerPayload,
      IReadOnlyDictionary<string, string> brokerMetadata,
      string messageId);
  }
}
