using BusinessLogicLayer.DTOs.Authentication;

namespace BusinessLogicLayer.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDto> RegisterWithPassword(CreateUserDto dto, CancellationToken ct = default);
        Task<TokenResponseDto> LoginAsyncByEmail(LoginByEmailDtoRequest dto, CancellationToken ct = default);
    }
}
