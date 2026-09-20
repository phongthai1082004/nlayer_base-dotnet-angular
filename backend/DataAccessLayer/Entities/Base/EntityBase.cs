using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Entities.Base
{
    public class EntityBase : IDentity<int>, ICreationAudit, IModificationAudit
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
