using DataAccessLayer.Interfaces.IEntities;

namespace DataAccessLayer.Entities
{
    public class GuidEntityBase : IDentity<Guid>, IAuditable, ISoftDelete
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public DateTime CreatedAt { get; set; } 
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
