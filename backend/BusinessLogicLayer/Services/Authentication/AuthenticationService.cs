using BusinessLogicLayer.Interfaces.Authentication;
using DataAccessLayer.Constants.Config;
using DataAccessLayer.Constants.Enums;
using DataAccessLayer.Constants.Exceptions;
using DataAccessLayer.Constants.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces.IRepositories.Common;
using BusinessLogicLayer.DTOs.Authentication;

namespace BusinessLogicLayer.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            JwtSettings jwtSettings)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings;
        }
        public async Task<TokenResponseDto> RegisterWithPassword(CreateUserDto dto, CancellationToken ct = default)
        {
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var _tokenRepository = _unitOfWork.GenerateRepository<RefreshToken, int>();

            if (await _userRepository.AnyWhereAsync(x => x.Email == dto.Email, ct))
            {
                throw new AppException(StatusCodes.Status409Conflict, AuthenticationMessages.EmailExists);
            }
            var user = await _userRepository.AddAsync(new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            }, ct);

            string accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email);
            string refreshToken = _jwtService.GenerateRefreshToken();

            await _tokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                Status = RefreshTokenStatus.Active,
            }, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return new TokenResponseDto(accessToken, refreshToken);
        }
        public async Task<TokenResponseDto> LoginAsyncByEmail(LoginByEmailDtoRequest dto, CancellationToken ct = default)
        {
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var _tokenRepository = _unitOfWork.GenerateRepository<RefreshToken, int>();

            var existingUser = await _userRepository.FindAsync(x => x.Email == dto.Email, ct);
            if (existingUser == null)
            {
                throw new AppException(StatusCodes.Status404NotFound, AuthenticationMessages.UserNotFound);
            } else
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, existingUser.PasswordHash))
                {
                    throw new AppException(StatusCodes.Status401Unauthorized, AuthenticationMessages.PasswordMismatch);
                }
            }

            string accessToken = _jwtService.GenerateToken(existingUser.Id.ToString(), existingUser.Email);
            string refreshToken = _jwtService.GenerateRefreshToken();

            await _tokenRepository.AddAsync(new RefreshToken
            {
                UserId = existingUser.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                Status = RefreshTokenStatus.Active,
            }, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return new TokenResponseDto(accessToken, refreshToken);
        }
    
    }
}
