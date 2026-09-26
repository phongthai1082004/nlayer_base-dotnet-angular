namespace BusinessLogicLayer.Interfaces.Authentication
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, string role);
        string GenerateRefreshToken();
    }
}
