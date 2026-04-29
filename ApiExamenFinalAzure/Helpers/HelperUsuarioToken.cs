using ApiExamenFinalAzure.Helpers;
using ApiExamenFinalAzure.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiExamenFinalAzure.Helpers
{
    public class HelperUsuarioToken
    {
        private IHttpContextAccessor contextAccessor;
        public HelperUsuarioToken(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public UserModel GetUsuario()
        {
            Claim claim =
                this.contextAccessor.HttpContext.User
                .FindFirst(z => z.Type == "UserData");
            string json = claim.Value;
            string jsonUsuario =
                HelperCryptography.DescifrarString(json);
            UserModel model = JsonConvert
                .DeserializeObject<UserModel>(jsonUsuario);
            return model;
        }
    }
}
