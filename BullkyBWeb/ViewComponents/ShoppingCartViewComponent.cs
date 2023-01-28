
using BullkyB.DataAccess.Repository;
using BullkyB.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utility;

namespace BullkyBWeb.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWrok _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWrok unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claims!=null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart)!=null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.
                    GetAll(u => u.ApplicationUserId == claims.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));

                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
