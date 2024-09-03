using System.ComponentModel.DataAnnotations;

namespace PlanifiqueAPI.Application.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Nome { get; set; }
        public string Color { get; set; }
    }
}
