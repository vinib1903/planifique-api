﻿using Microsoft.AspNetCore.Identity;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using PlanifiqueAPI.Infraestructure.Data;

namespace PlanifiqueAPI.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
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

            // Inicia a transação
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await _userManager.CreateAsync(user, registerDto.Password);

                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return result;
                    }

                  
                    var defaultCategories = new List<Category>
                    {
                        new Category { Nome = "Locomoção", UserId = user.Id },
                        new Category { Nome = "Moradia", UserId = user.Id },
                        new Category { Nome = "Alimentação", UserId = user.Id }
                    };

                    _context.Categories.AddRange(defaultCategories);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _emailService.SendWelcomeEmailAsync(registerDto.Email, registerDto.Nome);

                    //return Ok("Usuário registrado com sucesso!");

                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
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

            //if (!string.IsNullOrEmpty(updateDto.Password))
            //{
            //    if (updateDto.Password != updateDto.RePassword)
            //    {
            //        return IdentityResult.Failed(new IdentityError { Description = "As senhas não coincidem." });
            //    }

            //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //    var result = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);
            //    if (!result.Succeeded)
            //    {
            //        return result;
            //    }

            //    hasChanges = true;
            //}

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

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            if (newPassword != confirmPassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "As novas senhas não coincidem." });
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result;
        }

    }
}
