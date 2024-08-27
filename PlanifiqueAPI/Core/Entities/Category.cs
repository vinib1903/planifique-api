using System.ComponentModel.DataAnnotations;

namespace PlanifiqueAPI.Core.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
