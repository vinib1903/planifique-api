using PlanifiqueAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PlanifiqueAPI.Infraestructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurando a precisão do campo Valor
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Valor)
                .HasPrecision(18, 2);

            // Configurando a relação entre Transaction e Category
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions) // Atualizado para usar a propriedade de navegação Transactions
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.NoAction); // Configura DeleteBehavior como NoAction
        }
    }
}
