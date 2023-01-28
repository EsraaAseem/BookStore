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
    public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.orderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderSataus, string? paymentStatus=null)
        {
            var obj = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
             if(obj!=null)
            {
                obj.OrderStatus = orderSataus;
                if(paymentStatus!=null)
                {
                    obj.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePayment(int id, string sessionId, string paymentIntend)
        {
            var obj = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            obj.PaymentDate = DateTime.Now;
            obj.SessionId =sessionId;
            obj.PaymentIntentId = paymentIntend;

        }
    }
}
