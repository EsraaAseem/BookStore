using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BullkyBWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin )]
    public class coverType : Controller
    {
       private readonly IUnitOfWrok _unitOfWrok;
        public coverType(IUnitOfWrok unitOfWrok)
        {
            _unitOfWrok = unitOfWrok;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int id)
        {
            coverTypeMV coverTypemv = new()
            {
                coverType = new(),
            };
            if (id == 0)
            {
                return View(coverTypemv);

            }
            else
            {
                coverTypemv.coverType = _unitOfWrok.coverType.GetFirstOrDefualt(u => u.Id == id);
                return View(coverTypemv);

            }
        }
        [HttpPost]
        public IActionResult Upsert(coverTypeMV coverTypemv)
        {
            if(ModelState.IsValid)
            {
                if (coverTypemv.coverType.Id== 0)
                {
                    _unitOfWrok.coverType.Add(coverTypemv.coverType);
                    TempData["success"] = "data created succesfully";
                }
                else
                {
                    _unitOfWrok.coverType.Update(coverTypemv.coverType) ;
                    TempData["success"] = "data updated succesfully";
                }
                _unitOfWrok.save();
                return RedirectToAction("Index");
            }
            return View(coverTypemv);

        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypeAll = _unitOfWrok.coverType.GetAll();
            return Json(new { data = coverTypeAll });
        }
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWrok.coverType.GetFirstOrDefualt(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Faild To Deleting Data" });
            }
            _unitOfWrok.coverType.remove(obj);
            _unitOfWrok.save();
            return Json(new { success = true, Message = "Deleting Data successfully" });


        }
        #endregion
    }
}
