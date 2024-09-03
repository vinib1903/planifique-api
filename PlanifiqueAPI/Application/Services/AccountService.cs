using Microsoft.AspNetCore.Identity;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Core.Entities;
using PlanifiqueAPI.Core.Interfaces;
using PlanifiqueAPI.Infraestructure.Data;

namespace PlanifiqueAPI.Application.Services
{
    public class AccountService : IAccountService // implementação de interface
    {

        //injeção de dependências
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

        // método para criar/registrar um novo usuário, recebendo como parâmetro um objeto do tipo registerDto
        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerDto)
        {
            // verifica se a senha e a confirmação da senha recebidas como parâmetro são iguais
            if (registerDto.Password != registerDto.RePassword)
            {
                // sendo diferentes, retorna um erro
                return IdentityResult.Failed(new IdentityError { Description = "As senhas não coincidem." });
            }

            // se iguais, recebe os parâmetros na variável user para a criação do usuário
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Nome = registerDto.Nome,
                FotoPerfil = registerDto.FotoPerfil
            };

            // inicia a transação
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // faz a criação do usuário através do Identity e armazena o resultado na variável result
                    var result = await _userManager.CreateAsync(user, registerDto.Password);

                    // verifica o retorno da operação
                    if (!result.Succeeded)
                    {
                        // se for diferente de succeeded, desfaz as alterações
                        await transaction.RollbackAsync();
                        return result;
                    }

                  // se result = succeeded, cria as categorias padrão do usuário
                    var defaultCategories = new List<Category>
                    {
                        new Category { Nome = "Locomoção", UserId = user.Id },
                        new Category { Nome = "Moradia", UserId = user.Id },
                        new Category { Nome = "Alimentação", UserId = user.Id }
                    };


                    // adiciona as categorias criadas ao banco de dados e salva a operação
                    _context.Categories.AddRange(defaultCategories);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();


                    // envia o email de boas vindas ao usuário
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

        // método para logar um usuário recebendo como parâmetro um objeto do tipo loginDto
        public async Task<SignInResult> LoginUserAsync(LoginUserDto loginDto)
        {
            // faz o login através do Identity
            return await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
        }

        // método para deletar uma conta recebendo como parâmetro um id
        public async Task<IdentityResult> DeleteAccountAsync(string userId)
        {
            // recebe o id do usuário como parâmetro e recupera ele na variáver user
            var user = await _userManager.FindByIdAsync(userId);

            // verufica se encontrou um usuário com o respectivo id
            if (user == null)
            {
                // se não, retorna um erro
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            // se sim, deleta através do Identity
            return await _userManager.DeleteAsync(user);
        }

        // método para atualizar um usuário recebendo como parâmetro um objeto do tipo updateDto e um id
        public async Task<IdentityResult> UpdateUserAsync(string userId, UpdateUserDto updateDto)
        {
            // recebe o id do usuário como parâmetro e recupera ele na variáver user
            var user = await _userManager.FindByIdAsync(userId);

            // verufica se encontrou um usuário com o respectivo id
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            // define hasChanges como false (sem alterações)
            bool hasChanges = false;

            // compara o parâmetro nome do banco de dados com o recebido para atualização para verificar se houveram mudanças
            if (!string.IsNullOrEmpty(updateDto.Nome) && updateDto.Nome != user.Nome)
            {
                // se diferentes, atualiza e altera a variável hasChanges para true (houveram alterações)
                user.Nome = updateDto.Nome;
                hasChanges = true;
            }

            // compara o parâmetro email do banco de dados com o recebido para atualização para verificar se houveram mudanças
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
            {
                // se diferentes, atualiza e altera a variável hasChanges para true (houveram alterações)
                user.Email = updateDto.Email;

                // atualiza o UserName para refletir o novo email
                user.UserName = updateDto.Email;
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

            // compara o parâmetro foto de perfil do banco de dados com o recebido para atualização para verificar se houveram mudanças
            if (!string.IsNullOrEmpty(updateDto.FotoPerfil) && updateDto.FotoPerfil != user.FotoPerfil)
            {
                // se diferentes, atualiza e altera a variável hasChanges para true (houveram alterações)
                user.FotoPerfil = updateDto.FotoPerfil;
                hasChanges = true;
            }

            // verifica se houveram alterações (true/false)
            if (hasChanges)
            {
                // se sim, atualiza o banco de dados
                var updateResult = await _userManager.UpdateAsync(user);
                return updateResult;
            }

            // se não, apenas retorna success
            return IdentityResult.Success;
        }

        // método para alterar a senha de um usuário recebendo como parâmetro um id, a senha atual, a nova senha e a confirmação da nova senha
        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string confirmPassword)
        {
            // recebe o id do usuário como parâmetro e recupera ele na variáver user
            var user = await _userManager.FindByIdAsync(userId);

            // verufica se encontrou um usuário com o respectivo id
            if (user == null)
            {
                // se for nulo, retorna um erro
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            // se não, verifica se a nova senha é igual a confirmação da nova senha
            if (newPassword != confirmPassword)
            {
                // se diferentes, retorna um erro
                return IdentityResult.Failed(new IdentityError { Description = "As novas senhas não coincidem." });
            }

            // se iguais, faz a alteração usando o Identity
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result;
        }

    }
}
