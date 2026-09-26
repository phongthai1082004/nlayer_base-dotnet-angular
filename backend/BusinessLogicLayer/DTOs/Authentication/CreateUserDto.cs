namespace BusinessLogicLayer.DTOs.Authentication
{
    public sealed record CreateUserDto(
        string Email,
        string Password
    );
}
