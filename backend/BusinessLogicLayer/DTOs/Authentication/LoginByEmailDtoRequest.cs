namespace BusinessLogicLayer.DTOs.Authentication
{
    public record LoginByEmailDtoRequest(
        string Email,
        string Password
    );
}
