namespace AccountService.Dtos
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public required string AccountType { get; set; }
        public required decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
