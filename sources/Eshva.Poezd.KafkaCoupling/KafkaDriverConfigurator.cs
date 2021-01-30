#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class KafkaDriverConfigurator
  {
    public KafkaDriverConfigurator([NotNull] KafkaDriverConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public KafkaDriverConfigurator WithConsumerConfig([NotNull] ConsumerConfig consumerConfig)
    {
      _configuration.ConsumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));
      return this;
    }

    public KafkaDriverConfigurator WithProducerConfig([NotNull] ProducerConfig producerConfig)
    {
      _configuration.ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
      return this;
    }

    public KafkaDriverConfigurator WithCommitPeriod(long commitPeriod)
    {
      _configuration.CommitPeriod = commitPeriod > 0
        ? commitPeriod
        : throw new ArgumentOutOfRangeException(nameof(commitPeriod), "Commit period should be greater than 0.");

      return this;
    }

    public KafkaDriverConfigurator WithHeaderValueParser<THeaderValueParser>()
    {
      _configuration.HeaderValueParserType = typeof(THeaderValueParser);
      return this;
    }

    private readonly KafkaDriverConfiguration _configuration;
  }
}