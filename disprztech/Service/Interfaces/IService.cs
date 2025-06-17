namespace disprztech.Service.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByEmailAsync(string email);
        Task CreateAsync(T entity);
        Task BulkInsertAsync(IEnumerable<T> entities, string uniqueColumn);
        Task UpdateAsync(string key, T entity);
        Task DeleteAsync(int id);
    }
}
