namespace OnConcert.DAL.Generic
{
    public interface IReadOnlyRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> GetById(int id);
    }
}
