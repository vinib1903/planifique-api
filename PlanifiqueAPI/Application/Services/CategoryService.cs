using Microsoft.EntityFrameworkCore;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using PlanifiqueAPI.Infraestructure.Data;

namespace PlanifiqueAPI.Application.Services
{
    public class CategoryService : ICategoryService // implementação da interface
    {

        // injeção de dependência
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        // método para recuperar as categorias do usuário, recebendo como parâmetro seu id
        public async Task<IEnumerable<ReadCategoryDto>> GetCategoriesAsync(string userId)
        {
            // acessa a tabela Categories do banco de dados, filtrando pelo id do usuário que está fazendo a requisição
            return await _context.Categories
                .Where(c => c.UserId == userId)

                // recupera as informações de id, nome e cor das categorias e exibe em forma de lista (json)
                .Select(c => new ReadCategoryDto { Id = c.Id, Nome = c.Nome, Color = c.Color })
                .ToListAsync();
        }

        // método para recuperar uma categoria específica usando o seu id
        public async Task<ReadCategoryDto> GetCategoryByIdAsync(int id, string userId)
        {
            // acessa a tabela Categories do banco de dados e recupera a primeira categoria que coincide com o id do usuário requerinte e o id da categoria que foi passado como parâmetro
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return null;

            // recupera as informações de id, nome e cor da categoria
            return new ReadCategoryDto { Id = category.Id, Nome = category.Nome, Color = category.Color };
        }

        // método para criar uma categoria recebendo como parâmetro um id de usuário que está fazendo a requisição e um categoryDto
        public async Task<ReadCategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, string userId)
        {
            var category = new Category
            {
                Nome = categoryDto.Nome,
                UserId = userId,
                Color = categoryDto.Color
            };

            // adiciona ao banco de dados e salva as alterações
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new ReadCategoryDto { Id = category.Id, Nome = category.Nome };
        }

        // atualiza uma categoria recebendo como parâmetro um id de categoria, id do usuário que fez a requisição e um createCategoryDto
        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, string userId)
        {

            // acessa a tabela Categories do banco de dados e recupera a primeira categoria que coincide com o id do usuário requerinte e o id da categoria que foi passado como parâmetro
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return false;

            // atualiza os valores de nome e cor
            category.Nome = updateCategoryDto.Nome;
            category.Color = updateCategoryDto.Color;

            // atualiza e salva no banco de dados
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return true;
        }

        // deleta uma categoria do banco de dados recebendo como parâmetro o id do usuário que fez a requisição e o id da categoria
        public async Task<bool> DeleteCategoryAsync(int id, string userId)
        {
            // acessa a tabela Categories do banco de dados e recupera a primeira categoria que coincide com o id do usuário requerinte e o id da categoria que foi passado como parâmetro
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

