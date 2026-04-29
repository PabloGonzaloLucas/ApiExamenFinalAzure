using ApiExamenFinalAzure.Helpers;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiExamenFinalAzure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;
        private HelperUsuarioToken helper;

        public CubosController(RepositoryCubos repo, HelperUsuarioToken helper)
        {
            this.repo = repo;
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
    }
}
