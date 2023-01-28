using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BullkyBWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin )]

    public class Company : Controller
    {
       private readonly IUnitOfWrok _unitOfWrok;
        public Company(IUnitOfWrok unitOfWrok)
        {
            _unitOfWrok = unitOfWrok;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int id)
        {
            CompanyMV companymv = new()
            {
                company = new(),
            };
            if (id == 0)
            {
                return View(companymv);

            }
            else
            {
                 companymv.company = _unitOfWrok.Company.GetFirstOrDefualt(u => u.Id == id);
                return View(companymv);

            }
        }
        [HttpPost]
        public IActionResult Upsert(CompanyMV companyMV)
        {
            if(ModelState.IsValid)
            {
                if (companyMV.company.Id == 0)
                {
                    _unitOfWrok.Company.Add(companyMV.company);
                    TempData["success"] = "data created succesfully";
                }
                else
                {
                    _unitOfWrok.Company.Update(companyMV.company);
                    TempData["success"] = "data updated succesfully";
                }
                _unitOfWrok.save();
                return RedirectToAction("Index");
            }
            return View(companyMV);

        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var CompanyAll = _unitOfWrok.Company.GetAll();
            return Json(new { data = CompanyAll });
        }
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWrok.Company.GetFirstOrDefualt(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Faild To Deleting Data" });
            }
            _unitOfWrok.Company.remove(obj);
            _unitOfWrok.save();
            return Json(new { success = true, Message = "Deleting Data successfully" });


        }
        #endregion
    }
}
