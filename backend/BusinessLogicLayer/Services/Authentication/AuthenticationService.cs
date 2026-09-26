using BusinessLogicLayer.Interfaces.Authentication;
using DataAccessLayer.Constants.Config;
using DataAccessLayer.Constants.Enums;
using DataAccessLayer.Constants.Exceptions;
using DataAccessLayer.Constants.Messages;
using DataAccessLayer.Externals.Google;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces.IRepositories.Common;
using BusinessLogicLayer.DTOs.Authentication;
using FluentValidation;

namespace BusinessLogicLayer.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly IValidator<CreateUserDto> _createUserValidator;
        private readonly IValidator<LoginByEmailDtoRequest> _loginValidator;
        private readonly IGoogleOAuthClient _googleClient;
        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            JwtSettings jwtSettings,
            IValidator<CreateUserDto> createUserValidator,
            IValidator<LoginByEmailDtoRequest> loginValidator,
            IGoogleOAuthClient googleClient)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings;
            _createUserValidator = createUserValidator;
            _loginValidator = loginValidator;
            _googleClient = googleClient;
        }
        public async Task<TokenResponseDto> RegisterWithPasswordAsync(CreateUserDto dto, CancellationToken ct = default)
        {
            await _createUserValidator.ValidateAndThrowAsync(dto, ct);

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

            string accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role.ToString());
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
        public async Task<TokenResponseDto> LoginByEmailAsync(LoginByEmailDtoRequest dto, CancellationToken ct = default)
        {
            await _loginValidator.ValidateAndThrowAsync(dto, ct);

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

            string accessToken = _jwtService.GenerateToken(existingUser.Id.ToString(), existingUser.Email, existingUser.Role.ToString());
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
        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var _tokenRepository = _unitOfWork.GenerateRepository<RefreshToken, int>();
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var existingToken = await _tokenRepository.FindAsync(x => x.Token == refreshToken && x.Status == RefreshTokenStatus.Active, ct);
            if (existingToken == null || existingToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new AppException(StatusCodes.Status401Unauthorized, AuthenticationMessages.InvalidRefreshToken);
            }
            var user = await _userRepository.FindAsync(u => u.Id == existingToken.UserId, ct);
            if (user == null)
            {
                throw new AppException(StatusCodes.Status404NotFound, AuthenticationMessages.UserNotFound);
            }
            string newAccessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role.ToString());
            string newRefreshToken = _jwtService.GenerateRefreshToken();
            existingToken.Status = RefreshTokenStatus.Revoked;
            _tokenRepository.Update(existingToken);
            await _tokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                Status = RefreshTokenStatus.Active,
            }, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return new TokenResponseDto(newAccessToken, newRefreshToken);
        }
        public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            var _tokenRepository = _unitOfWork.GenerateRepository<RefreshToken, int>();
            var existingToken = await _tokenRepository.FindAsync(x => x.Token == refreshToken && x.Status == RefreshTokenStatus.Active, ct);
            if (existingToken != null)
            {
                existingToken.Status = RefreshTokenStatus.Revoked;
                _tokenRepository.Update(existingToken);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }
        public async Task<TokenResponseDto> LoginByGoogleCodeAsync(string code, CancellationToken ct = default)
        {
            var g = await _googleClient.ExchangeCodeAsync(code, ct);
            return await LoginOrCreateExternalAsync("Google", g.Subject, g.Email, g.AccessToken, g.RefreshToken, g.ExpiresAt, ct);
        }
        public async Task<TokenResponseDto> LoginOrCreateExternalAsync(string provider, string providerKey, string email, string? googleAccessToken = null, string? googleRefreshToken = null, DateTime? googleExpiresAt = null, CancellationToken ct = default)
        {
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var _extRepository = _unitOfWork.GenerateRepository<ExternalToken, int>();
            var _tokenRepository = _unitOfWork.GenerateRepository<RefreshToken, int>();

            var linked = await _extRepository.FindAsync(x => x.Provider == provider && x.ProviderKey == providerKey, ct);
            User? user = null;
            if (linked != null)
            {
                user = await _userRepository.FindAsync(u => u.Id == linked.UserId, ct);
                if (user != null && googleAccessToken != null)
                {
                    linked.AccessToken = googleAccessToken;
                    if (googleRefreshToken != null) linked.RefreshToken = googleRefreshToken;
                    if (googleExpiresAt != null) linked.ExpiresAt = googleExpiresAt;
                    _extRepository.Update(linked);
                }
            }
            if (user == null)
            {
                user = await _userRepository.FindAsync(u => u.Email == email, ct);
                if (user == null)
                {
                    user = await _userRepository.AddAsync(new User
                    {
                        Email = email,
                        PasswordHash = string.Empty
                    }, ct);
                }
                if (linked == null)
                {
                    await _extRepository.AddAsync(new ExternalToken
                    {
                        UserId = user.Id,
                        Provider = provider,
                        ProviderKey = providerKey,
                        AccessToken = googleAccessToken,
                        RefreshToken = googleRefreshToken,
                        ExpiresAt = googleExpiresAt
                    }, ct);
                }
            }

            string accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role.ToString());
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
        public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var user = await _userRepository.FindAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                throw new AppException(StatusCodes.Status404NotFound, AuthenticationMessages.UserNotFound);
            }
            return user;
        }
        public async Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var _userRepository = _unitOfWork.GenerateRepository<User, Guid>();
            var user = await _userRepository.FindAsync(u => u.Email == email, ct);
            if (user == null)
            {
                throw new AppException(StatusCodes.Status404NotFound, AuthenticationMessages.UserNotFound);
            }
            return user;
        }
    }
}
