using ApiExamenFinalAzure.Helpers;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiExamenFinalAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private RepositoryUsuarios repoUsuarios;
        private HelperUsuarioToken helper;

        public CubosController(RepositoryCubos repo, RepositoryUsuarios repoUsuarios, HelperUsuarioToken helper)
        {
            this.repo = repo;
            this.repoUsuarios = repoUsuarios;
            this.helper = helper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> GetCubos()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            return Ok(cubos);
        }

        [HttpGet]
        [Route("[action]/{marca}")]
        public async Task<ActionResult> GetCubosByMarca(string marca)
        {
            List<Cubo> cubos = await this.repo.FindCubosByMarcaAsync(marca);
            return Ok(cubos);
        }

        [HttpPost]
        [Authorize]
        [Route("[action]/{idCubo:int}")]
        public async Task<ActionResult> ComprarCubo(int idCubo)
        {
            UserModel usuario = this.helper.GetUsuario();
            await this.repoUsuarios.ComprarCubo(idCubo, usuario.IdUsuario);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<ActionResult<List<CompraCubo>>> PedidosUsuario()
        {
            UserModel usuario = this.helper.GetUsuario();
            List<CompraCubo> pedidos = await this.repoUsuarios.PedidosAsync(usuario.IdUsuario);
            return Ok(pedidos);
        }
    }
}
