using ApiExamenFinalAzure.Helpers;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiExamenFinalAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryUsuarios repo;
        private HelperActionOAuthService helper;
        private HelperUsuarioToken helperTokens;

        public AuthController(RepositoryUsuarios repo, HelperActionOAuthService helper, HelperUsuarioToken helperTokens)
        {
            this.repo = repo;
            this.helper = helper;
            this.helperTokens = helperTokens;
        }

        [HttpPost]
        [Route("[action]")]

        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario usuario = await this.repo.LoginUsuario(model.Nombre, model.Password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                UserModel userModel = new UserModel
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Imagen = usuario.Imagen
                };

                string jsonEmpleado =
                    JsonConvert.SerializeObject(userModel);
                string jsonCypher =
                    HelperCryptography.CifrarString(jsonEmpleado);

                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonCypher),
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );

                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler()
                    .WriteToken(token)
                });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Register(Usuario model)
        {
            await this.repo.CreateUsuarioAsync( model.Nombre, 
                model.Email, model.Pass, model.Imagen);
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public ActionResult<UserModel> PerfilUsuario()
        {
            UserModel model = this.helperTokens.GetUsuario();
            return model;
        }


    }
}
