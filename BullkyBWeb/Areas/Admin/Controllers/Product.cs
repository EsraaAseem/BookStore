using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.Model;
using BullkyB.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utility;

namespace BullkyBWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class Product : Controller
    {
       private readonly IUnitOfWrok _unitOfWrok;
        private readonly IWebHostEnvironment _hostEnvironment;
        public Product(IUnitOfWrok unitOfWrok, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWrok = unitOfWrok;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int id)
        {
            ProductMV productmv = new()
            {
                Product = new(),
                CategoryList = _unitOfWrok.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                }),
                coverTypeList = _unitOfWrok.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.coverName,
                    Value = i.Id.ToString(),
                })
            };
            if (id == 0)
            {
                return View(productmv);

            }
            else
            {
                productmv.Product = _unitOfWrok.Product.GetFirstOrDefualt(u => u.Id == id);
                return View(productmv);

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductMV productmv,IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Imgs\products");
                    var extention = Path.GetExtension(file.FileName);
                    if (productmv.Product.ImageUrl != null)
                    {
                        var oldimg = Path.Combine(wwwRootPath, productmv.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productmv.Product.ImageUrl = @"\Imgs\products\" + fileName + extention;
                }
                //var WWWRoutePath = _hostEnvironment.WebRootPath;
                //if(file!=null)
                //{
                //    var FileName = Guid.NewGuid().ToString();
                //    var upload = Path.Combine(WWWRoutePath, @"Product\Images");
                //    var Exaintion = Path.GetExtension(file.FileName);
                //    if(productmv.Product.ImageUrl!=null)
                //    {
                //        var oldImg = Path.Combine(WWWRoutePath, productmv.Product.ImageUrl.TrimStart('\\'));
                //        if (System.IO.File.Exists(oldImg))
                //        {
                //            System.IO.File.Delete(oldImg);
                //        }
                //    }
                //    using (var fileStream =new FileStream( Path.Combine(upload, FileName + Exaintion), FileMode.Create))
                //    {
                //        file.CopyTo(fileStream);
                //    }
                //    productmv.Product.ImageUrl = @"\Product\Images" + FileName + Exaintion;
                //}
                if (productmv.Product.Id == 0)
                {
                    _unitOfWrok.Product.Add(productmv.Product);
                    TempData["success"] = "data created succesfully";
                }
                else
                {
                    _unitOfWrok.Product.Update(productmv.Product);
                    TempData["success"] = "data updated succesfully";
                }
                _unitOfWrok.save();
                return RedirectToAction("Index");
            }
                Console.WriteLine("not valid");
            return View(productmv);

        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var ProductAll = _unitOfWrok.Product.GetAll(includeProperity:"category,CoverType");
            return Json(new { data = ProductAll });
        }
        [HttpDelete]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWrok.Product.GetFirstOrDefualt(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, Message = "Faild To Deleting Data" });
            }
            var oldImg = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }

            _unitOfWrok.Product.remove(obj);
            _unitOfWrok.save();
            return Json(new { success = true, Message = "Deleting Data successfully" });


        }
        #endregion
    }
}
