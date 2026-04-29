using ApiExamenFinalAzure.Data;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiExamenFinalAzure.Repositories
{
    public class RepositoryCubos
    {
        private ServiceStorageBlobs serviceBlobs;
        private CubosContext context;
        private KeyVaultAccesorModel secrets;

        public RepositoryCubos(CubosContext context, ServiceStorageBlobs serviceBlobs, KeyVaultAccesorModel secrets)
        {
            this.context = context;
            this.secrets = secrets;
            this.serviceBlobs = serviceBlobs;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            List<Cubo> cubos = await this.context.Cubos.ToListAsync();
            foreach (Cubo cubo in cubos)
            {
                if (!string.IsNullOrEmpty(cubo.Imagen))
                {
                    cubo.Imagen = this.secrets.BlobsBaseUrl + "/containerexamenfree/" + cubo.Imagen;
                }
            }
            return cubos;
        }

        public async Task<List<Cubo>> FindCubosByMarcaAsync(string marca)
        {
            List<Cubo> cubos = await this.context.Cubos
                .Where(x => x.Marca == marca)
                .ToListAsync();

            foreach (Cubo cubo in cubos)
            {
                if (!string.IsNullOrEmpty(cubo.Imagen))
                {
                    cubo.Imagen = this.secrets.BlobsBaseUrl + "/containerexamenfree/" + cubo.Imagen;
                }
            }

            return cubos;
        }
    }
}
