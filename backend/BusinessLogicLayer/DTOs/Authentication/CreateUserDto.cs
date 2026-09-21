namespace BusinessLogicLayer.DTOs.Authentication
{
    public record CreateUserDto(
        string Email,
        string? Password
    );
}
