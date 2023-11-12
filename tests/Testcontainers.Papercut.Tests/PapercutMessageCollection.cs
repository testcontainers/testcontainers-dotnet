namespace Testcontainers.Papercut
{
    public class PapercutMessageCollection
    {
        public int TotalMessageCount { get; set; }
        public IEnumerable<PapercutMessageSummary> Messages { get; set; }
    }
}