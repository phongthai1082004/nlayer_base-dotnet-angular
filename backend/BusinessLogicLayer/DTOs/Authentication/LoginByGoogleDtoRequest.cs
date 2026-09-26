namespace BusinessLogicLayer.DTOs.Authentication
{
    public sealed record LoginByGoogleDtoRequest(
        string Code
    );
}
