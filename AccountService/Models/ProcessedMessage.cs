
using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class ProcessedMessage
    {
        [Key]
        public Guid MessageId { get; internal set; }
        public DateTime ProcessedAt { get; internal set; }
        public string? Handler { get; internal set; }
    }
}
