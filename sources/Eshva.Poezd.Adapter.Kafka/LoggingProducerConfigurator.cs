#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class LoggingProducerConfigurator : IProducerConfigurator
  {
    public LoggingProducerConfigurator([NotNull] ILogger<LoggingProducerConfigurator> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ProducerBuilder<TKey, TValue> Configure<TKey, TValue>(
      [NotNull] ProducerBuilder<TKey, TValue> builder,
      [NotNull] ISerializer<TKey> keySerializer,
      [NotNull] ISerializer<TValue> valueSerializer)
    {
      if (builder == null) throw new ArgumentNullException(nameof(builder));
      if (keySerializer == null) throw new ArgumentNullException(nameof(keySerializer));
      if (valueSerializer == null) throw new ArgumentNullException(nameof(valueSerializer));

      return builder
        .SetKeySerializer(keySerializer)
        .SetValueSerializer(valueSerializer)
        .SetLogHandler(LogHandler)
        .SetErrorHandler(ErrorHandler)
        .SetStatisticsHandler(StatisticsHandler);
    }

    private void StatisticsHandler<TKey, TValue>(IProducer<TKey, TValue> producer, string statistics)
    {
      _logger.LogInformation("Producer statistics: {Statistics}", statistics);
    }

    private void LogHandler<TKey, TValue>(IProducer<TKey, TValue> producer, LogMessage logMessage)
    {
      _logger.LogDebug(
        "Producing to Kafka. Client: {Client}, syslog level: {LogLevel}, message: {Message}.",
        logMessage.Name,
        logMessage.Level,
        logMessage.Message);
    }

    private void ErrorHandler<TKey, TValue>(IProducer<TKey, TValue> producer, Error error)
    {
      if (!error.IsFatal)
        _logger.LogWarning("Producer error: {Error}. No action required.", error);
      else
      {
        _logger.LogError("Fatal error producing to Kafka. Error: {Error}.", error.Reason);
        throw new KafkaException(error);
      }
    }

    private readonly ILogger<LoggingProducerConfigurator> _logger;
  }
}
