using DataAccessLayer.Entities.Base;

namespace DataAccessLayer.Entities
{
    public class ExternalToken : EntityBase
    {
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string? ProviderDisplayName { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public User User { get; set; } = null!;
    }
}
