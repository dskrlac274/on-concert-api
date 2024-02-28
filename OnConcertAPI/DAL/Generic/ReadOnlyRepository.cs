using Microsoft.EntityFrameworkCore;

namespace OnConcert.DAL.Generic
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
    {
        protected readonly OnConcertContext _context;

        public ReadOnlyRepository(OnConcertContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll() =>
            _context.Set<T>().AsNoTracking();

        public async Task<T> GetById(int id)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
