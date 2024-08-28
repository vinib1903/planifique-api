namespace PlanifiqueAPI.Core.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }

        // Foreign keys
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
