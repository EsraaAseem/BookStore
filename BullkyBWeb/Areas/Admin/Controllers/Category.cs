using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BullkyBWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class Category : Controller
    {
       private readonly IUnitOfWrok _unitOfWrok;
        public Category(IUnitOfWrok unitOfWrok)
        {
            _unitOfWrok = unitOfWrok;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int id)
        {
            CategoryMV categorymv = new()
            {
                category = new(),
            };
            if (id == 0)
            {
                return View(categorymv);

            }
            else
            {
                 categorymv.category = _unitOfWrok.Category.GetFirstOrDefualt(u => u.Id == id);
                return View(categorymv);

            }
        }
        [HttpPost]
        public IActionResult Upsert(CategoryMV categorymv)
        {
            if(ModelState.IsValid)
            {
                if (categorymv.category.Id == 0)
                {
                    _unitOfWrok.Category.Add(categorymv.category);
                    TempData["success"] = "data created succesfully";
                }
                else
                {
                    _unitOfWrok.Category.Update(categorymv.category);
                    TempData["success"] = "data updated succesfully";
                }
                _unitOfWrok.save();
                return RedirectToAction("Index");
            }
            return View(categorymv);

        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var CategoryAll = _unitOfWrok.Category.GetAll();
            return Json(new { data = CategoryAll });
        }
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWrok.Category.GetFirstOrDefualt(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Faild To Deleting Data" });
            }
            _unitOfWrok.Category.remove(obj);
            _unitOfWrok.save();
            return Json(new { success = true, Message = "Deleting Data successfully" });


        }
        #endregion
    }
}
