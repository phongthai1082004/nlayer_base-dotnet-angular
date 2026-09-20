namespace DataAccessLayer.Interfaces.IEntities
{
    public interface IModificationAudit
    {
        DateTime? ModifiedAt { get; set; }
        Guid? ModifiedBy { get; set; }
    }
}
