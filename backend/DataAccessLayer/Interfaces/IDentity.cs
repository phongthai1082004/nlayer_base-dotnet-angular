namespace DataAccessLayer.Interfaces
{
    public interface IDentity<TKey>
    {
        TKey Id { get; set; }
    }
}
