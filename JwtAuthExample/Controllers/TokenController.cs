using System.Threading.Tasks;
using JwtAuthExample.Repository;
using JwtAuthExample.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthExample.Controllers
{
    [Route("api/token")]
    public class TokenController : Controller
    {

        private readonly ITokenRepository _tokenRepository;

        public TokenController(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        // Get Token
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationModel model)
        {
            var token = await _tokenRepository.GetToken(model);
            return Ok(token);
        }

        // Refresh Token
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenModel model)
        {
            var token = _tokenRepository.RefreshToken(model);
            return Ok(token);
        }

        // Revoke Token
        [HttpPost("revoke")]
        public IActionResult Revoke([FromBody] RevokeTokenModel model)
        {
            _tokenRepository.RevokeToken(model.RefreshToken);
            return Ok();
        }


    }
}
