
using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class ProcessedMessage
    {
        [Key]
        public Guid MessageId { get; internal set; }

        [Required]
        public DateTime ProcessedAt { get; internal set; }
        [Required]
        public string? Handler { get; internal set; }
    }
}
