using ApplicationCore.Interfaces;
using Infrastructure.Context;
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


        public IEnumerable<T> GetAll()
        {
            return _table.ToList();
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public T Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
