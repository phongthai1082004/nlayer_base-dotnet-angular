namespace BusinessLogicLayer.DTOs.Authentication
{
    public sealed record LoginByEmailDtoRequest(
        string Email,
        string Password
    );
}
