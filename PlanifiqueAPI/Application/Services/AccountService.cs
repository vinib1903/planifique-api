using Microsoft.AspNetCore.Identity;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;

namespace PlanifiqueAPI.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerDto)
        {
            if (registerDto.Password != registerDto.RePassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "As senhas não coincidem." });
            }

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Nome = registerDto.Nome,
                FotoPerfil = registerDto.FotoPerfil
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            return result;
        }

        public async Task<SignInResult> LoginUserAsync(LoginUserDto loginDto)
        {
            return await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            bool hasChanges = false;

            if (!string.IsNullOrEmpty(updateDto.Nome) && updateDto.Nome != user.Nome)
            {
                user.Nome = updateDto.Nome;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
            {
                user.Email = updateDto.Email;
                user.UserName = updateDto.Email; // Atualiza o UserName para refletir o novo e-mail
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                if (updateDto.Password != updateDto.RePassword)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "As senhas não coincidem." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
                if (!result.Succeeded)
                {
                    return result;
                }

                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(updateDto.FotoPerfil) && updateDto.FotoPerfil != user.FotoPerfil)
            {
                user.FotoPerfil = updateDto.FotoPerfil;
                hasChanges = true;
            }

            if (hasChanges)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                return updateResult;
            }

            // Se não houve alterações, retorna um sucesso sem modificações
            return IdentityResult.Success;
        }


    }
}
