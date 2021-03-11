#region Usings

using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_caching_producer_registry
  {
    [Fact]
    public void when_create_not_registered_producer_it_should_return_new_producer_instance()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      sut.Get<int, int>(new ProducerConfig()).Should().NotBeNull()
        .And.Subject.Should().BeAssignableTo<IProducer<int, int>>("new instance should be created");
    }

    [Fact]
    public void when_create_already_registered_producer_it_should_return_the_same_producer_instance()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      var producerConfig = new ProducerConfig();
      var producer1 = sut.Get<int, int>(producerConfig);
      var producer2 = sut.Get<int, int>(producerConfig);
      producer2.Should().BeSameAs(producer1, "same instance should be return");
    }

    [Fact]
    public void when_create_producers_with_different_configs_and_same_key_and_value_types_it_should_return_different_producers()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      var producer1 = sut.Get<int, int>(new ProducerConfig());
      var producer2 = sut.Get<int, int>(new ProducerConfig());
      producer2.Should().NotBeSameAs(producer1, "for different configs different producers should be return");
    }

    [Fact]
    public void when_create_producers_with_same_config_and_key_type_but_different_value_type_it_should_return_different_producers()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      var producerConfig = new ProducerConfig();
      var producer1 = sut.Get<int, string>(producerConfig);
      var producer2 = sut.Get<int, byte[]>(producerConfig);
      producer2.Should().NotBeSameAs(producer1, "for different value types different producers should be return");
    }

    [Fact]
    public void when_create_producers_with_same_config_and_value_type_but_different_key_type_it_should_return_different_producers()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      var producerConfig = new ProducerConfig();
      var producer1 = sut.Get<string, byte[]>(producerConfig);
      var producer2 = sut.Get<Null, byte[]>(producerConfig);
      producer2.Should().NotBeSameAs(producer1, "for different key types different producers should be return");
    }

    [Fact]
    public void when_dispose_it_should_dispose_all_producers()
    {
      var sut = new CachingProducerRegistry(new TestProducerFactory(new Dictionary<string, object>()));
      var producerConfig = new ProducerConfig();
      var producers = new[]
      {
        (ITestableProducer) sut.Get<string, byte[]>(producerConfig),
        (ITestableProducer) sut.Get<Null, byte[]>(producerConfig),
        (ITestableProducer) sut.Get<string, byte[]>(producerConfig)
      };

      sut.Dispose();

      producers.Count(producer => producer.IsDisposed).Should().Be(expected: 3, "all producers should be disposed");
    }
  }
}