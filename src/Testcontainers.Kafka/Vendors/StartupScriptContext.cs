namespace Testcontainers.Kafka.Vendors;

internal class StartupScriptContext
{
    public KafkaContainer Container { get; set; } = null!;
    public KafkaConfiguration Configuration { get; set; } = null!;
}