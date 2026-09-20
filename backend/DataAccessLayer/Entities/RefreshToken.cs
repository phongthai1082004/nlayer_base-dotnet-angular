using DataAccessLayer.Entities.Base;

namespace DataAccessLayer.Entities
{
    public class RefreshToken : EntityBase
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public User User { get; set; } = null!;
    }
}
