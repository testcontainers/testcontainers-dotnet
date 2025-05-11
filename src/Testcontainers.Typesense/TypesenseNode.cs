namespace Testcontainers.Typesense
{
    public class TypesenseNode
    {
        public Uri BaseAddress { get; set; }

        public string Protocol { get; set; }

        public string Host { get; set; }

        public string Port { get; set; }

        public string ApiKey { get; set; }
    }
}
