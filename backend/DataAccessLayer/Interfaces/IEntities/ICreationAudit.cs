namespace DataAccessLayer.Interfaces.IEntities
{
    public interface ICreationAudit
    {
        DateTime CreatedAt { get; set; }
        Guid CreatedBy { get; set; }
    }
}
