#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of an ingress API.
  /// </summary>
  public interface IIngressApi
  {
    /// <summary>
    /// Gets the ingress API configuration.
    /// </summary>
    [NotNull]
    IngressApiConfiguration Configuration { get; }

    /// <summary>
    /// Gets the ingress pipe fitter.
    /// </summary>
    [NotNull]
    IPipeFitter PipeFitter { get; }

    /// <summary>
    /// Gets the message type registry.
    /// </summary>
    [NotNull]
    IIngressMessageTypesRegistry MessageTypesRegistry { get; }

    /// <summary>
    /// Gets the handlers registry.
    /// </summary>
    [NotNull]
    IHandlerRegistry HandlerRegistry { get; }

    /// <summary>
    /// Gets queue name patterns.
    /// </summary>
    /// <returns>
    /// List of queue name patterns.
    /// </returns>
    [NotNull]
    IEnumerable<string> GetQueueNamePatterns();
  }
}