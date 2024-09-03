using Microsoft.AspNetCore.Mvc;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using PlanifiqueAPI.Infraestructure.Data;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace PlanifiqueAPI.Application.Services
{
    public class TransactionService : ITransactionService // implementa a interface
    {
        // injeção de dependência
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        // método para criar uma transação
        public async Task<ReadTransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto, string userId)
        {
            var transaction = new Transaction
            {
                Valor = transactionDto.Valor,
                Descricao = transactionDto.Descricao,
                Data = transactionDto.Data,
                CategoryId = transactionDto.CategoryId,
                UserId = userId
            };

            // adiciona ao banco de dados e salva a operação
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new ReadTransactionDto
            {
                Id = transaction.Id,
                Valor = transaction.Valor,
                Descricao = transaction.Descricao,
                Data = transaction.Data,
                CategoryId = transaction.CategoryId,
                CategoryNome = (await _context.Categories.FindAsync(transaction.CategoryId))?.Nome
            };
        }

        // recupera as transações do usuário
        public async Task<IEnumerable<ReadTransactionDto>> GetTransactionsAsync(string userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new ReadTransactionDto
                {
                    Id = t.Id,
                    Valor = t.Valor,
                    Descricao = t.Descricao,
                    Data = t.Data,
                    CategoryId = t.CategoryId,
                    CategoryNome = t.Category.Nome
                })
                .ToListAsync();
        }

        // recupera uma transação usando o seu id
        public async Task<ReadTransactionDto> GetTransactionByIdAsync(int id, string userId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null) return null;

            return new ReadTransactionDto
            {
                Id = transaction.Id,
                Valor = transaction.Valor,
                Descricao = transaction.Descricao,
                Data = transaction.Data,
                CategoryId = transaction.CategoryId,
                CategoryNome = transaction.Category.Nome
            };
        }

        // atualiza uma transação
        public async Task<bool> UpdateTransactionAsync(int id, CreateTransactionDto transactionDto, string userId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null) return false;

            transaction.Valor = transactionDto.Valor;
            transaction.Descricao = transactionDto.Descricao;
            transaction.Data = transactionDto.Data;
            transaction.CategoryId = transactionDto.CategoryId;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return true;
        }

        // remove uma transação do banco de dados
        public async Task<bool> DeleteTransactionAsync(int id, string userId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}