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

        // cria as tabelas do banco de dados
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // define o comportamento das entidades quando o modelo é criado (é usado quando o EF Core está construindo o modelo de dados com base nas entidades)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // configurando a precisão do campo Valor
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Valor) // propriedade valor na entidade transaction
                .HasPrecision(18, 2); // 18: quantidade de dígitos que o campo pode armazenar; 2: casas após a vírgula

            // configurando a relação entre transaction e category
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category) // uma transaction só possui uma categoria
                .WithMany(c => c.Transactions) // atualizado para usar a propriedade de navegação transactions (uma categoria pode ter muitas transações)
                .HasForeignKey(t => t.CategoryId) // o campo CategoryId em transections se refere ao campo Id de Categories
                .OnDelete(DeleteBehavior.NoAction); // configura DeleteBehavior como NoAction, evitando a deleção em cascata de entidades relacionadas
        }
    }
}
