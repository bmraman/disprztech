namespace disprztech.Service.Interfaces
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task BulkInsertAsync(IEnumerable<T> entities, string uniqueColumn);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
    }
}
