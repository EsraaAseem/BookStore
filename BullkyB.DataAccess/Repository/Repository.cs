using BullkeyBookPro.DataAccess.Repository.IRepository;
using BullkyB.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BullkeyBookPro.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T:class
    {
        ApplicationDbContext _db;
        public DbSet<T> dbSet;
        public Repository(ApplicationDbContext db) 
        {
            this._db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T identity)
        {
            _db.Add(identity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperity = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperity != null)
            {
                foreach (var properity in includeProperity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(properity);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefualt(Expression<Func<T, bool>> filter, string? includeProperity = null, bool tracked= true)
        {
            IQueryable<T> query;
            if (tracked == true)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            if (filter!=null)
            {
                query = query.Where(filter);
            }
            if (includeProperity != null)
            {
                foreach(var properity in includeProperity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(properity);
                }
            }
            return query.FirstOrDefault();
        }

        public void remove(T identity)
        {
            _db.Remove(identity);
        }

        public void removerange(IEnumerable<T> identity)
        {
            _db.RemoveRange(identity);
        }
    }
}
