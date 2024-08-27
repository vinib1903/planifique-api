using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanifiqueAPI.Application.DTOs;
using PlanifiqueAPI.Application.Services;
using PlanifiqueAPI.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlanifiqueAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, UserManager<User> userManager, ITokenService tokenService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            var result = await _accountService.RegisterUserAsync(registerDto);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Usuário registrado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var result = await _accountService.LoginUserAsync(loginDto);
            if (!result.Succeeded)
            {
                return Unauthorized("Credenciais inválidas.");
            }

            // Gerar o token 
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = _tokenService.GenerateToken(claims);

            //return Ok(new { Token = token });
            return Ok(token);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Usuário não está autenticado.");

            var result = await _accountService.DeleteAccountAsync(userId);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Conta excluída com sucesso.");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Usuário não está autenticado.");

            var result = await _accountService.UpdateUserAsync(userId, updateDto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Perfil atualizado com sucesso.");
        }
    }
}
