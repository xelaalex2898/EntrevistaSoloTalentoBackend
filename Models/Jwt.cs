using System.Security.Claims;

namespace EntrevistaSoloTalentoBackend.Models
{
    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public string subject { get; set; }

        public static dynamic ValidateToken(ClaimsIdentity identity)
        {
            response resp = new response();
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    resp.Body = "verificar token o token invalido";
                    resp.Codigo = 500;
                    resp.Error = true;
                    return resp;

                }

            }
            catch (Exception ex)
            {

                resp.Body = ex.Message;
                resp.Codigo = 500;
                resp.Error = true;
                return resp;

            }
            resp.Body = "verificar token o token invalido";
            resp.Codigo = 500;
            resp.Error = true;
            return resp;
        }
    }
    public class JwtResponse
    {
        public string token { get; set; } 
        public int code { get; set; }
    }
}
