using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private ApplicationDbContext _db;
        private DbSet<T> _table;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _table;
        }

        public void Delete(object id)
        {
            T existing = _table.Find(id);
            _table.Remove(existing);
        }

        public T Get(int id)
        {
            return _table.Find(id);
        }

        public void Insert(T entity)
        {
            _table.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }
    }
}
