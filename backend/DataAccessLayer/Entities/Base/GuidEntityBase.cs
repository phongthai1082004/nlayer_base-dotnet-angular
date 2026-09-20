using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Entities
{
    public class GuidEntityBase : IDentity<Guid>, ICreationAudit, IModificationAudit, ISoftDelete
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
