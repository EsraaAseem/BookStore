using BullkeyBookPro.DataAccess.Repository.IRepository;
using BullkyB.DataAccess.Data;
using BullkyB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullkyB.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        public void Update(OrderHeader orderHeader);
        public void UpdateStripePayment(int id,string sessionId,string paymentIntend);
        public void UpdateStatus(int id,string orderSataus,string? paymentStatus=null);

    }

}
