using Microsoft.IdentityModel.Tokens;
using NexusServergRPC.Services;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NexusServergRPC.Auth
{
    public class NexusTokenIssuer
    {
        private readonly IConfiguration _configuration;

        public NexusTokenIssuer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private NexusToken GenerateToken(string username)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Keys:JWT"]);

            return new NexusToken(Convert.ToBase64String(key), username, DateTime.Now.AddSeconds(10));
        }


        public NexusToken IssueToken(string username)
        {
            var token = GenerateToken(username);
            NexusService.Server?.AddToken(token);
            return token;
        }

        public bool ValidateToken(string token)
        {
            var _token = NexusService.Server?.Tokens.Find(x => x.Token == token);

            if (_token != null)
                return true;
            else
                return false;
        }

        public NexusToken? RetrieveToken(string username)
        {
            var _token = NexusService.Server?.Tokens.Find(x => x.UserName == username);

            return _token != null ? _token : null;
        }
    }
}
