using System.ComponentModel.DataAnnotations;

namespace AccountService.Dtos
{    public record TransactionResult
    {
        [Required]
        public required bool Success { get; set; }
        [Required]
        public required string Message { get; set; }
    }

}
