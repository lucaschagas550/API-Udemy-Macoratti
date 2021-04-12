using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APICatalago.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APICatalago.Controllers
{
    [Produces("application/json")] // define como padrão de retorno no swagger o formato json para este controlador
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration; // injeção de dependdecia do appsettings.json, permite ler o arquivo

        public AutorizaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config) //Injeção de dependencia 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;
        }
        
        [HttpGet]
        public ActionResult<string> Get() // para verificar se api esta atendendo
        {
            return "AutorizaController :: Acessado em : " + DateTime.Now.ToLongDateString();
        }

        //api/autoriza/register
        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="model">Um objeto UsuarioDTO</param>
        /// <returns>Status 200 e o token para o cliente</returns>
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UsuarioDTO model)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            //}

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true // deifiniu como true apenas para confirma o email valido, para n enviar solicitaçao e ter que confirma o email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);
            return Ok(GeraToken(model));
        }

        /// <summary>
        /// Verifica as credencias de um usuário
        /// </summary>
        /// <param name="userInfo">Um objeto do tipo UsuarioDTO</param>
        /// <returns>Status 200 e o token para o cliente</returns>
        /// <remarks>Retornar o status 200 e o token para novo </remarks>
        [HttpPost("login")]
        public async Task<ActionResult>Login([FromBody] UsuarioDTO userInfo)
        {

            var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false); // se tentar mais que 3x sera bloqueado

            if(result.Succeeded)
            {
                return Ok(GeraToken(userInfo));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return BadRequest(ModelState);
            }
        }

        private UsuarioTokenDTO GeraToken(UsuarioDTO userInfo)
        {
            //define declarações do usuario

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("meuPet", "pipoca"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //gera uma chave privada com base em um algoritmo simetrico

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            //gera a assinatura digital do token usando o algoritmo Hmac e a chave privada
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Tempo de expiração do token
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            //classe que representa um toke JWT e gera o token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["TokenConfiguration:Issuer"],
                audience: _configuration["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credenciais);

            //retorna os dados com token e informaçoes
            return new UsuarioTokenDTO()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Message = "Token JWT OK"
            };
        }
    }
}
