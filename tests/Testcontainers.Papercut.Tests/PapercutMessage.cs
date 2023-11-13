using System.Collections.Generic;
using JetBrains.Annotations;

namespace Testcontainers.Papercut
{
    public class PapercutMessage
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<EmailAddressDetail> From { get; set; }
        public IEnumerable<EmailAddressDetail> To { get; set; }
        public IEnumerable<EmailAddressDetail> Cc { get; set; }
        public IEnumerable<EmailAddressDetail> BCc { get; set; }
        [CanBeNull] public string HtmlBody { get; set; }
        [CanBeNull] public string TextBody { get; set; }
        public IEnumerable<HeaderDetail> Headers { get; set; }
        public IEnumerable<SectionDetail> Sections { get; set; }
    }
}