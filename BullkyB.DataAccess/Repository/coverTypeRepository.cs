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
    public class coverTypeRepository : Repository<coverType>, IcoverTypeRepository
    {
        private ApplicationDbContext _db;
        public coverTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(coverType coverType)
        {
            _db.Update(coverType);
        }
    }
}
