namespace DataAccessLayer.Interfaces
{
    public interface ICreationAudit
    {
        DateTime CreatedAt { get; set; }
        Guid CreatedBy { get; set; }
    }
}
