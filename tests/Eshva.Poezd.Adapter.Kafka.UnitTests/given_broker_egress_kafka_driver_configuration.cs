#region Usings

using System.Reflection;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_egress_kafka_driver_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_valid()
    {
      var sut = ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration();
      sut.Validate().Should().BeEmpty("configured with no errors");
    }

    [Fact]
    public void when_validating_it_should_validate_expected_number_of_properties()
    {
      const int expectedNumberOfValidatingProperties = 5;

      var properties = typeof(BrokerEgressKafkaDriverConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      properties.Should().HaveCount(
        expectedNumberOfValidatingProperties,
        $"Seams like you've added new properties to {nameof(BrokerEgressKafkaDriverConfiguration)}. " +
        $"Update its {nameof(IMessageRouterConfigurationPart.Validate)} method to test them or update " +
        $"{nameof(expectedNumberOfValidatingProperties)} const value.");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateBrokerEgressKafkaDriverConfigurationWithout(configuration => configuration.ProducerConfig = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressKafkaDriverConfigurationWithout(configuration => configuration.ProducerFactoryType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressKafkaDriverConfigurationWithout(configuration => configuration.HeaderValueCodecType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressKafkaDriverConfigurationWithout(configuration => configuration.ProducerConfiguratorType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressKafkaDriverConfigurationWithout(configuration => configuration.SerializerFactoryType = null)
        .Validate().Should().HaveCount(expected: 1);
    }
  }
}
