using BullkyB.DataAccess.Data;
using BullkyB.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.DataAccess.Repository
{
    public class UnitOfWrok : IUnitOfWrok
    {
       private ApplicationDbContext _db;
        public UnitOfWrok(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            coverType = new coverTypeRepository(db);
            Product = new ProductRepository(db);
            Company = new CompanyRepository(db);
            ApplicationUser = new ApplicationUserRepository(db);
            ShoppingCart = new ShoppingCartRepository(db);
            orderHeader = new OrderHeaderRepository(db);
            orderDetails = new OrderDetailRepository(db);
        }
        public ICategoryRepository Category { get; private set; }
        public IcoverTypeRepository coverType { get; private set; }
        public IProductRepository Product { get; set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository orderHeader { get; private set; }
        public IOrderDetailsRepository orderDetails { get; private set; }
        public void save()
        {
            _db.SaveChanges();
        }
    }
}
