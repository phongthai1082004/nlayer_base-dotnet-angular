namespace DataAccessLayer.Interfaces.IEntities
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        Guid? CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        Guid? ModifiedBy { get; set; }
    }
}
