namespace Tozawa.Bff.Portal.Services
{
   #nullable enable
    public interface IEntityService<TType>
    {
        Task<TType?> GetItem(Guid id);
        Task<TType?> GetItem(string code);
        Task<List<TType>> GetItems();
        void ClearCache();
        Task SortItems();

    }
}