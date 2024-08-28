using System.ComponentModel.DataAnnotations;

namespace PlanifiqueAPI.Application.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public decimal Valor { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
