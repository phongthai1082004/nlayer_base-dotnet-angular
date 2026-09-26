using BusinessLogicLayer.DTOs.Authentication;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDto> RegisterWithPasswordAsync(CreateUserDto dto, CancellationToken ct = default);
        Task<TokenResponseDto> LoginByEmailAsync(LoginByEmailDtoRequest dto, CancellationToken ct = default);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task LogoutAsync(string refreshToken, CancellationToken ct = default);
        Task<TokenResponseDto> LoginOrCreateExternalAsync(string provider, string providerKey, string email, string? googleAccessToken = null, string? googleRefreshToken = null, DateTime? googleExpiresAt = null, CancellationToken ct = default);
        Task<TokenResponseDto> LoginByGoogleCodeAsync(string code, CancellationToken ct = default);
        Task<User> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
        Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default);
    }
}
