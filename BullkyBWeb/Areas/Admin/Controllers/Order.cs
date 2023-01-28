using BullkyB.DataAccess.Repository;
using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Utility;

namespace BullkyBWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class Order : Controller
    {
        [BindProperty]
        public OrderMV orderMV { get; set; }
        private readonly IUnitOfWrok _unitOfWrok;
        
        public Order(IUnitOfWrok unitOfWrok)
        {
            _unitOfWrok = unitOfWrok;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int Id)
        {
            orderMV = new OrderMV()
            {
                OrderHeader = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id == Id, includeProperity: "ApplicationUser"),
                OrderDetails = _unitOfWrok.orderDetails.GetAll(u => u.OrderId == Id,includeProperity:"product")

            };
            return View(orderMV);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDetails()
        {
            var orderFormDb = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id ==orderMV.OrderHeader.Id, tracked:false);

            orderFormDb.Name = orderMV.OrderHeader.Name;
            orderFormDb.PhonrNumber = orderMV.OrderHeader.PhonrNumber;
            orderFormDb.StreetAdress = orderMV.OrderHeader.StreetAdress;
            orderFormDb.City = orderMV.OrderHeader.City;
            orderFormDb.State = orderMV.OrderHeader.State;
            orderFormDb.PostalCode = orderMV.OrderHeader.PostalCode;
            if (orderMV.OrderHeader.Carrier != null)
            {
                orderFormDb.Carrier = orderMV.OrderHeader.Carrier;

            }
            if (orderMV.OrderHeader.TrackingNumber != null)
            {
                orderFormDb.TrackingNumber = orderMV.OrderHeader.TrackingNumber;

            }
            _unitOfWrok.orderHeader.Update(orderFormDb);
            _unitOfWrok.save();
            TempData["success"] = "data Updated successfully";

            return RedirectToAction("Details", "Order", new { Id = orderFormDb.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWrok.orderHeader.UpdateStatus(orderMV.OrderHeader.Id,SD.StatusInProcess);
            _unitOfWrok.save();
            TempData["success"] = "data Updated successfully";

            return RedirectToAction("Details", "Order", new { Id = orderMV.OrderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult ShipedOrder()
        {
            var orderFormDb = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id == orderMV.OrderHeader.Id, tracked: false);
            orderFormDb.Carrier = orderMV.OrderHeader.Carrier;
            orderFormDb.TrackingNumber = orderMV.OrderHeader.TrackingNumber;
            orderFormDb.OrderStatus = SD.StatusShipped;
            if(orderFormDb.PaymentStatus==SD.PaymentStatusDelayPayment)
            {
                orderFormDb.PaymentBueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWrok.orderHeader.Update(orderFormDb);
            _unitOfWrok.save();
            TempData["success"] = "data Shipped successfully";

            return RedirectToAction("Details", "Order", new { Id = orderMV.OrderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult OrderCancel()
        {
            var orderFormDb = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id == orderMV.OrderHeader.Id, tracked: false);
            if(orderFormDb.PaymentStatus==SD.PaymentStatusApproved)
            {
                var option = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFormDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWrok.orderHeader.UpdateStatus(orderFormDb.Id, SD.StatusCancelled, SD.StatusRefunded);

            }
            else
            {
                _unitOfWrok.orderHeader.UpdateStatus(orderFormDb.Id, SD.StatusCancelled,SD.StatusCancelled);
            }
            _unitOfWrok.save();
            TempData["success"] = "data Cancled successfully";

            return RedirectToAction("Details", "Order", new { Id = orderMV.OrderHeader.Id });
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaymentDelay()
        {
            orderMV.OrderHeader = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id == orderMV.OrderHeader.Id, includeProperity: "ApplicationUser");
           orderMV.OrderDetails = _unitOfWrok.orderDetails.GetAll(u => u.OrderId == orderMV.OrderHeader.Id, includeProperity: "product");


                var domain = "http://localhost:52059/";
                var options = new SessionCreateOptions
                   {
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    SuccessUrl = domain + $"Admin/Order/ConfirmOrderDelay?id={orderMV.OrderHeader.Id}",
                    CancelUrl = domain + $"Admin/Order/Details?Id={orderMV.OrderHeader.Id}",
                  };
                foreach (var item in orderMV.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Title,
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                };
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWrok.orderHeader.UpdateStripePayment(orderMV.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWrok.save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

        }
      
        public IActionResult ConfirmOrderDelay(int id)
        {
            OrderHeader orderHeader = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id == id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWrok.orderHeader.UpdateStatus(id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWrok.save();

                }
            }
            
            return View(id);
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll(string OrderStatus)
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWrok.orderHeader.GetAll(includeProperity: "ApplicationUser");

             if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
             {
                 orderHeaders = _unitOfWrok.orderHeader.GetAll(includeProperity: "ApplicationUser");
             }
             else
             {
                 var claimIdentity = (ClaimsIdentity)User.Identity;
                 var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                 orderHeaders = _unitOfWrok.orderHeader.GetAll(u=>u.ApplicationUserId==claims.Value,includeProperity: "ApplicationUser");
             }

            /*switch (OrderStatus)
            {
                case "Pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "Processing":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "Shipped":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "Approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            */
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
