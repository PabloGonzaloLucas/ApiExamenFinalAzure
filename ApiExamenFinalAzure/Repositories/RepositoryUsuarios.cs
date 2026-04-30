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
            
            await this.context.Usuarios.AddAsync(usuario);
            await this.context.SaveChangesAsync();
        }
        public async Task<Usuario> LoginUsuario(string nombre, string pass)
        {
            return await this.context.Usuarios.Where(z => z.Nombre == nombre && z.Pass == pass).FirstOrDefaultAsync();
        }

        public async Task<List<CompraCubo>> PedidosAsync(int idUser)
        {
            return await this.context.Compras
                .Where(c => c.IdUsuario == idUser)
                .OrderByDescending(c => c.FechaPedido)
                .ToListAsync();
        }

        private async Task<int> GetMaxIdPedidoAsync()
        {
            if (!await this.context.Compras.AnyAsync())
            {
                return 1;
            }

            return await this.context.Compras.MaxAsync(c => c.IdPedido) + 1;
        }

        public async Task ComprarCubo(int idCubo, int idUsuario)
        {
            // Basic checks to avoid FK issues if the DB has constraints.
            bool existeUsuario = await this.context.Usuarios.AnyAsync(u => u.IdUsuario == idUsuario);
            if (!existeUsuario)
            {
                throw new InvalidOperationException($"El usuario {idUsuario} no existe.");
            }

            bool existeCubo = await this.context.Cubos.AnyAsync(c => c.IdCubo == idCubo);
            if (!existeCubo)
            {
                throw new InvalidOperationException($"El cubo {idCubo} no existe.");
            }

            CompraCubo compra = new CompraCubo
            {
                IdPedido = await this.GetMaxIdPedidoAsync(),
                IdCubo = idCubo,
                IdUsuario = idUsuario,
                FechaPedido = DateTime.UtcNow
            };

            await this.context.Compras.AddAsync(compra);
            await this.context.SaveChangesAsync();
        }
    }
}
