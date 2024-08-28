using System.ComponentModel.DataAnnotations;

namespace PlanifiqueAPI.Application.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        public string Nome { get; set; }
        public string Color { get; set; }
    }
}
