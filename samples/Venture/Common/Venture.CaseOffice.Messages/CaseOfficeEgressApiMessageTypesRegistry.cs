#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Pipeline;
using FlatSharp;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.CaseOffice.Messages.V1.Events;

#endregion

namespace Venture.CaseOffice.Messages
{
  public sealed class CaseOfficeEgressApiMessageTypesRegistry : EgressApiMessageTypesRegistry
  {
    public override void Initialize()
    {
      _routeMap = RouteMap.GetRouteMap();

      var messageTypes = GetType().Assembly.ExportedTypes
        .Where(type => type.FullName!.StartsWith("Venture.CaseOffice.Messages.V1.", StringComparison.InvariantCulture));

      foreach (var messageType in messageTypes)
      {
        if (!_routeMap.TryGetValue(messageType, out var descriptor))
          throw new InvalidOperationException($"Can not create a descriptor for message type {messageType.FullName}.");

        AddDescriptor(
          messageType.FullName!,
          messageType,
          descriptor);
      }
    }

    private IDictionary<Type, object> _routeMap;

    private class Descriptor<TMessage> : IEgressApiMessageTypeDescriptor<TMessage>
      where TMessage : class
    {
      public Descriptor(string queueName, Func<TMessage, object> getKey)
      {
        _getKey = getKey;
        _queueNames.Add(queueName);
      }

      public IReadOnlyCollection<string> QueueNames => _queueNames.AsReadOnly();

      public object GetKey(TMessage message) => _getKey(message);

      public int Serialize(TMessage message, Memory<byte> buffer) => FlatBufferSerializer.Default.Serialize(message, buffer.Span);

      private readonly Func<TMessage, object> _getKey;

      private readonly List<string> _queueNames = new List<string>(capacity: 1);
    }

    private static class RouteMap
    {
      public static IDictionary<Type, object> GetRouteMap()
      {
        const string officeCommands = "case.commands.case-office.v1";
        const string justiceCaseFacts = "case.facts.justice-case.v1";
        const string researchCaseFacts = "case.facts.research-case.v1";
        return new Dictionary<Type, object>
        {
          {typeof(CreateJusticeCase), new Descriptor<CreateJusticeCase>(officeCommands, message => Guid.NewGuid().ToByteArray())},
          {typeof(CreateResearchCase), new Descriptor<CreateResearchCase>(officeCommands, message => Guid.NewGuid().ToByteArray())},
          {typeof(JusticeCaseCreated), new Descriptor<JusticeCaseCreated>(justiceCaseFacts, message => message.CaseId.value)},
          {typeof(ResearchCaseCreated), new Descriptor<ResearchCaseCreated>(researchCaseFacts, message => message.CaseId.value)}
        };
      }
    }
  }
}
