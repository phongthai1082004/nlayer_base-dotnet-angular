using DataAccessLayer.Interfaces.IEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace DataAccessLayer.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateAuditEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateAuditEntities(DbContext? dbContext)
        {
            if (dbContext == null) return;

            // Lấy ID user an toàn (nếu có)
            Guid? currentUserId = null;
            var userIdStr = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var parsedGuid))
            {
                currentUserId = parsedGuid;
            }

            var currentTime = DateTime.UtcNow;

            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditable.CreatedAt = currentTime;
                        auditable.CreatedBy = currentUserId; 
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.ModifiedAt = currentTime;
                        auditable.ModifiedBy = currentUserId; 
                    }
                }

                if (entry.Entity is ISoftDelete softDelete)
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        softDelete.IsDeleted = true;
                        softDelete.DeletedAt = currentTime;
                        softDelete.DeletedBy = currentUserId; // Có thể null (System)
                    }
                }
            }
        }
    }
}
