using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BullkyB.Model.ViewModel
{
    public class ShoppingCartMV
    {
        [ValidateNever]
        public IEnumerable<ShoppingCart> CartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
