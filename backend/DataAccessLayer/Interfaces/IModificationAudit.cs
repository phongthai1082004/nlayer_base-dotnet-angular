namespace DataAccessLayer.Interfaces
{
    public interface IModificationAudit
    {
        DateTime? ModifiedAt { get; set; }
        Guid? ModifiedBy { get; set; }
    }
}
