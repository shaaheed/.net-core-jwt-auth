using JwtAuthExample.ViewModels;
using System.Threading.Tasks;

namespace JwtAuthExample.Repository
{
    public interface ITokenRepository
    {
        Task<TokenViewModel> GetToken(AuthenticationModel model);

        TokenViewModel RefreshToken(RefreshTokenModel model);

        void RevokeToken(string refreshToken);

    }
}
