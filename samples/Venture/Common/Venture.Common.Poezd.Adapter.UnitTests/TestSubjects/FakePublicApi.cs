#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public class FakePublicApi : IIngressPublicApi
  {
    public IngressPublicApiConfiguration Configuration { get; set; }

    public IPipeFitter PipeFitter { get; set; }

    public IIngressMessageTypesRegistry MessageTypesRegistry { get; set; }

    public IHandlerRegistry HandlerRegistry { get; set; }

    public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
  }
}
