namespace DineroBank.Shared.DTOs.Transaction
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public Guid AccountId { get; set; }                
        public Guid? TargetAccountId { get; set; }         
        public decimal Amount { get; set; }
        public string Type { get; set; } // CREDIT | DEBIT | TRANSFER
        // public string Currency { get; set; } = "EUR";
        public DateTime CreatedAt { get; set; } 
        public string? Description { get; set; }     
    }
}
