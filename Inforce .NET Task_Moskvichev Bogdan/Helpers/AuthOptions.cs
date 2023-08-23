using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Helpers
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; //видавець токена
        public const string AUDIENCE = "MyAuthClient"; //споживач токена
        const string KEY = "mysupersecret_secretkey!123";   //ключ для шифрації
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
