using DataAccessLayer.Interfaces.IEntities;

namespace DataAccessLayer.Entities.Base
{
    public class EntityBase : IDentity<int>, IAuditable
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } 
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
