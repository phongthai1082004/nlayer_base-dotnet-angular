namespace BusinessLogicLayer.DTOs.Authentication
{
    public record TokenResponseDto(
        string AccessToken,
        string RefreshToken
    );
}
