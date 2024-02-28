namespace OnConcert.DAL.Generic
{
    public class Repository<T> : ReadOnlyRepository<T>, IRepository<T> where T : class
    {
        public Repository(OnConcertContext context) : base(context) { }

        public void Add(T entity) =>
            _context.Add(entity);

        public void Update(T entity) =>
            _context.Update(entity);

        public void Delete(T entity) =>
            _context.Remove(entity);

        public async Task Save() =>
            await _context.SaveChangesAsync();
    }
}
