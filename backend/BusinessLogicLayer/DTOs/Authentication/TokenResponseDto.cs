namespace BusinessLogicLayer.DTOs.Authentication
{
    public sealed record TokenResponseDto(
        string AccessToken,
        string RefreshToken
    );
}
