namespace PlanifiqueAPI.Application.DTOs
{
    public class ReadTransactionDto
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public int CategoryId { get; set; }
        public string CategoryNome { get; set; }
    }
}
