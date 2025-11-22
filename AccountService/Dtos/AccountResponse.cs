namespace AccountService.Dtos
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
