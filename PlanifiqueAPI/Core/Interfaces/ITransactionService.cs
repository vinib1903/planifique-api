using PlanifiqueAPI.Application.DTOs;

namespace PlanifiqueAPI.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<ReadTransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto, string userId);
        Task<IEnumerable<ReadTransactionDto>> GetTransactionsAsync(string userId);
        Task<ReadTransactionDto> GetTransactionByIdAsync(int id, string userId);
        Task<bool> UpdateTransactionAsync(int id, CreateTransactionDto transactionDto, string userId);
        Task<bool> DeleteTransactionAsync(int id, string userId);
    }
}
