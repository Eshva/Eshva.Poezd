#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  internal class TestIngressEnterPipeline : IPipeFitter
  {
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
    {
      throw new NotImplementedException();
    }
  }
}
