#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithKafkaDriver(
      this BrokerIngressConfigurator ingress,
      Action<IngressKafkaDriverConfigurator> configurator)
    {
      var configuration = new IngressKafkaDriverConfiguration();
      configurator(new IngressKafkaDriverConfigurator(configuration));
      IIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(new IngressKafkaDriver(configuration));
      return ingress;
    }
  }
}
