using DataAccessLayer.Constants.Enums;
using DataAccessLayer.Entities.Base;

namespace DataAccessLayer.Entities
{
    public class RefreshToken : EntityBase
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public RefreshTokenStatus Status { get; set; } = RefreshTokenStatus.Active;

        public void RefreshStatus()
        {
            if (Status == RefreshTokenStatus.Active && DateTime.UtcNow >= ExpiresAt)
                Status = RefreshTokenStatus.Expired;
        }

        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
            Status = RefreshTokenStatus.Revoked;
        }

        public User User { get; set; } = null!;
    }
}
