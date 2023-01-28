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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
       public int Increase(ShoppingCart cart, int Count)
        {
            cart.Count += Count;
            return cart.Count;
        }
        public int Decrease(ShoppingCart cart, int Count)
        {
            cart.Count -= Count;
            return cart.Count;
        }


    }
}
