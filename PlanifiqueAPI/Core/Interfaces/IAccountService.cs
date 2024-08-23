using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace PlanifiqueAPI.Application.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerDto);
        Task<SignInResult> LoginUserAsync(LoginUserDto loginDto);
        Task<IdentityResult> DeleteAccountAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateDto);
    }
}