using BullkyB.DataAccess.Repository;
using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Utility;

namespace BullkyBWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class Cart : Controller
    {
       private readonly IUnitOfWrok _unitOfWrok;
        [BindProperty]
        public ShoppingCartMV ShoppingCartMV { get; set; }
        public Cart(IUnitOfWrok unitOfWrok)
        {
            _unitOfWrok = unitOfWrok;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartMV = new ShoppingCartMV()
            {
              CartList=_unitOfWrok.ShoppingCart.GetAll(u=>u.ApplicationUserId==claims.Value,includeProperity:"product"),
              OrderHeader=new(),
            };
            foreach(var cart in ShoppingCartMV.CartList)
            {
                cart.Price = getPrice(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartMV.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartMV);
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartMV = new ShoppingCartMV()
            {
                CartList = _unitOfWrok.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperity: "product"),
                OrderHeader = new(),
            };
            ShoppingCartMV.OrderHeader.ApplicationUser = _unitOfWrok.ApplicationUser.GetFirstOrDefualt(u => u.Id == claims.Value);
            ShoppingCartMV.OrderHeader.Name = ShoppingCartMV.OrderHeader.ApplicationUser.Name;
            ShoppingCartMV.OrderHeader.City = ShoppingCartMV.OrderHeader.ApplicationUser.City;
            ShoppingCartMV.OrderHeader.StreetAdress = ShoppingCartMV.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartMV.OrderHeader.PhonrNumber = ShoppingCartMV.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartMV.OrderHeader.PostalCode = ShoppingCartMV.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartMV.OrderHeader.State = ShoppingCartMV.OrderHeader.ApplicationUser.State;

            foreach (var cart in ShoppingCartMV.CartList)
            {
                cart.Price = getPrice(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartMV.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartMV);
        }

        [HttpPost]
        [ActionName("Summary")]

        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartMV.CartList = _unitOfWrok.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperity: "product");

            ShoppingCartMV.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartMV.OrderHeader.ApplicationUserId = claims.Value;
           
            foreach (var cart in ShoppingCartMV.CartList)
            {
                cart.Price = getPrice(cart.Count, cart.product.Price, cart.product.Price50, cart.product.Price100);
                ShoppingCartMV.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            ApplicationUser applicationUser = _unitOfWrok.ApplicationUser.GetFirstOrDefualt(u => u.Id == claims.Value);
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartMV.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartMV.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                ShoppingCartMV.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartMV.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
            }
            _unitOfWrok.orderHeader.Add(ShoppingCartMV.OrderHeader);
            _unitOfWrok.save();
            foreach(var cart in ShoppingCartMV.CartList)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartMV.OrderHeader.Id,
                    Count = cart.Count,
                    price = cart.Price,
                };
                _unitOfWrok.orderDetails.Add(orderDetails);
                _unitOfWrok.save();
            }
                if (applicationUser.CompanyId.GetValueOrDefault() == 0)
                {
                    var domain = "http://localhost:52059/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                   
                    Mode = "payment",
                    SuccessUrl = domain+$"Customer/Cart/ConfirmOrder?id={ShoppingCartMV.OrderHeader.Id}",
                    CancelUrl = domain+$"Customer/Cart/Index",
                };
                foreach(var item in ShoppingCartMV.CartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price*100),
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
                _unitOfWrok.orderHeader.UpdateStripePayment(ShoppingCartMV.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWrok.save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
        
            else
            {
                return RedirectToAction("ConfirmOrder", "Cart", new { id = ShoppingCartMV.OrderHeader.Id });
            }



            /*_unitOfWrok.ShoppingCart.removerange(ShoppingCartMV.CartList);
            _unitOfWrok.save();
            return RedirectToAction("Index");*/
        }


        public double getPrice(int Quantity,double price ,double price50,double price100)
        {
            if(Quantity <=50)
            {
                return price;
            }
            else
            {
                if(Quantity<=100)
                {
                    return price50;
                }
                else
                {
                    return price100;
                }
            }
        }
        public IActionResult ConfirmOrder(int id)
        {
              OrderHeader orderHeader = _unitOfWrok.orderHeader.GetFirstOrDefualt(u => u.Id==id);
              if (orderHeader.PaymentStatus != SD.PaymentStatusDelayPayment)
              {
                  var service = new SessionService();
                  Session session = service.Get(orderHeader.SessionId);

                  if (session.PaymentStatus.ToLower() == "paid")
                  {
                    _unitOfWrok.orderHeader.UpdateStripePayment(id, orderHeader.SessionId, session.PaymentIntentId);
                    HttpContext.Session.Clear();
                    _unitOfWrok.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                      _unitOfWrok.save();

                  }
              }
              List<ShoppingCart> shoppingCarts = _unitOfWrok.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
              _unitOfWrok.ShoppingCart.removerange(shoppingCarts);
              _unitOfWrok.save();
              return View(id);
        }
        public IActionResult Plus(int Id)
        {
            var shopping = _unitOfWrok.ShoppingCart.GetFirstOrDefualt(u => u.Id == Id);
            _unitOfWrok.ShoppingCart.Increase(shopping, 1);
            _unitOfWrok.save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int Id)
        {
            var shopping = _unitOfWrok.ShoppingCart.GetFirstOrDefualt(u => u.Id == Id);
            if(shopping.Count<=1)
            {
                _unitOfWrok.ShoppingCart.remove(shopping);
              var count= _unitOfWrok.ShoppingCart.GetAll(u => u.ApplicationUserId == shopping.ApplicationUserId).ToList().Count-1;
                HttpContext.Session.SetInt32(SD.SessionCart,count);

            }
            else
            {
                _unitOfWrok.ShoppingCart.Decrease(shopping, 1);

            }
            _unitOfWrok.save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int Id)
        {
            var shopping = _unitOfWrok.ShoppingCart.GetFirstOrDefualt(u => u.Id == Id);
            _unitOfWrok.ShoppingCart.remove(shopping);
            _unitOfWrok.save();
            var count = _unitOfWrok.ShoppingCart.GetAll(u => u.ApplicationUserId == shopping.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.SessionCart, count);

            return RedirectToAction(nameof(Index));
        }
    }
}
