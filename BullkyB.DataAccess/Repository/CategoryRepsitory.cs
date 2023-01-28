using BullkeyBookPro.DataAccess.Repository;
using BullkyB.DataAccess.Data;
using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Category category)
        {
            _db.Update(category);
        }
    }
}
