namespace TransactionService.Dtos
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid? TargetAccountId { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
