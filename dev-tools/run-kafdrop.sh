connectionString="${1:-"localhost:9092"}"
hostPortNumber="${2:-9000}"

echo "Starting Kafdrop, a Kafka Web UI tool."
echo "It will be connected to Kafka broker at $connectionString"
docker run -d -p $hostPortNumber:9000 \
    -e KAFKA_BROKERCONNECT=$connectionString \
    --name kafdrop \
    obsidiandynamics/kafdrop

echo "Kafdrop is started and accessible at http://localhost:$hostPortNumber"
