using ApiExamenFinalAzure.Data;
using ApiExamenFinalAzure.Models;
using ApiExamenFinalAzure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ApiExamenFinalAzure.Repositories
{
    public class RepositoryUsuarios
    {
        private ServiceStorageBlobs serviceBlobs;
        private CubosContext context;
        private KeyVaultAccesorModel secrets;

        public RepositoryUsuarios(CubosContext context, ServiceStorageBlobs serviceBlobs, KeyVaultAccesorModel secrets)
        {
            this.context = context;
            this.secrets = secrets;
            this.serviceBlobs = serviceBlobs;
        }

        public async Task<Usuario> FindUsuarioAsync(string email)
        {
            Usuario usuario = await this.context.Usuarios
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();
            return usuario;
        }

        public async Task<int> GetMaxIdUsuario()
        {
            if (this.context.Usuarios.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Usuarios.MaxAsync(z => z.IdUsuario) + 1;
            }

        }

        public async Task CreateUsuarioAsync( string nombre,  string email, string pass, string foto)
        {
            foto = secrets.BlobsBaseUrl + "containerexamenfree/" + foto;
            Usuario usuario = new Usuario
            {
                IdUsuario = await this.GetMaxIdUsuario(),
                Nombre = nombre,
                Email = email,
                Pass = pass,
                Imagen = foto
            };

            //if (foto != null && foto.Length > 0)
            //{
            //    string blobname = foto.FileName;
            //    using (Stream stream = foto.OpenReadStream())
            //    {
            //        await this.serviceStorage.UploadBlobAsync("marcasprueba", blobname, stream);
            //    }

            
            //serviceBlobs.UploadBlobAsync
            await this.context.Usuarios.AddAsync(usuario);
            await this.context.SaveChangesAsync();
        }
        public async Task<Usuario> LoginUsuario(string nombre, string pass)
        {
            return await this.context.Usuarios.Where(z => z.Nombre == nombre && z.Pass == pass).FirstOrDefaultAsync();
        }
    }
}
