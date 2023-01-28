using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BullkyB.Model.ViewModel
{
    public class OrderMV
    {
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
