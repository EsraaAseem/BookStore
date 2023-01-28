using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Utility;

namespace BullkyBWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IUnitOfWrok _unitOfWrk;
        public HomeController(ILogger<HomeController> logger,IUnitOfWrok unitOfWork)
        {
            _unitOfWrk = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> product = _unitOfWrk.Product.GetAll(includeProperity:"category,CoverType");
            return View(product);
        }
        public IActionResult Details(int? productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Count=1,
                ProductId= productId.Value,
                product=_unitOfWrk.Product.GetFirstOrDefualt(u=>u.Id== productId, includeProperity: "category,CoverType")
            };

            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claims.Value;
            ShoppingCart cart = _unitOfWrk.ShoppingCart.GetFirstOrDefualt(u => u.ApplicationUserId == claims.Value && u.ProductId == shoppingCart.ProductId);
            if (cart == null)
            {
                _unitOfWrk.ShoppingCart.Add(shoppingCart);
                _unitOfWrk.save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWrk.ShoppingCart.
                    GetAll(u => u.ApplicationUserId == claims.Value).ToList().Count);

            }
            else
            {
                _unitOfWrk.ShoppingCart.Increase(cart, shoppingCart.Count);
                _unitOfWrk.save();
            }
            return RedirectToAction("index");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}