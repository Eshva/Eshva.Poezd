#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressPublicApiConfigurator
  {
    public EgressPublicApiConfigurator(EgressPublicApiConfiguration configuration)
    {
      _configuration = configuration;
    }

    public EgressPublicApiConfigurator WithId(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    public EgressPublicApiConfigurator WithPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.PipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressPublicApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly EgressPublicApiConfiguration _configuration;
  }
}