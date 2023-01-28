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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            var obj = _db.Products.FirstOrDefault(u => u.Id == product.Id);
            if(obj!=null)
            {
                obj.Title = product.Title;
                obj.Author = product.Author;
                obj.Description = product.Description;
                obj.ISBN = product.ISBN;
                obj.Price = product.Price;
                obj.ListPrice = product.ListPrice;
                obj.Price50 = product.Price50;
                obj.Price100 = product.Price100;
                obj.CategoryId = product.CategoryId;
                obj.coveTypeId = product.coveTypeId;
                if (product.ImageUrl != null)
                {
                    obj.ImageUrl = product.ImageUrl;
                }
            }

        }
    }
}
