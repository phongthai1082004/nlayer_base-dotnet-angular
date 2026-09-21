using DataAccessLayer.Constants.Enums;

namespace DataAccessLayer.Entities
{
    public class User : GuidEntityBase
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? GoogleId { get; set; }
        public Role Role { get; set; } = Role.User;
        public bool IsActive { get; set; } = true;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
