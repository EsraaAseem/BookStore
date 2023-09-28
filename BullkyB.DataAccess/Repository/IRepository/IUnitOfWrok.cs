using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.DataAccess.Repository.IRepository
{
    public interface IUnitOfWrok
    {
        ICategoryRepository Category {get;}
        IcoverTypeRepository coverType {get;}
        IProductRepository Product {get;}
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository orderHeader { get; }
        IOrderDetailsRepository orderDetails { get; }

        public void save();
    }
}
