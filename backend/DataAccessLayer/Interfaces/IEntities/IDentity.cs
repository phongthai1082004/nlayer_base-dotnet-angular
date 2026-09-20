namespace DataAccessLayer.Interfaces.IEntities
{
    public interface IDentity<TKey>
    {
        TKey Id { get; set; }
    }
}
