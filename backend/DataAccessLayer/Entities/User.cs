using DataAccessLayer.Constants.Enums;

namespace DataAccessLayer.Entities
{
    public class User : GuidEntityBase
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
        public bool IsActive { get; set; } = true;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<ExternalToken> ExternalTokens { get; set; } = new List<ExternalToken>();
    }
}
