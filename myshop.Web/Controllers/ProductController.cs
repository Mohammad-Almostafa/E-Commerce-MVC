using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Business.DTOs;
using myshop.Business.Services.Interfaces;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;
using myshop.Web.ViewModels;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOperations")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileService fileService;

        public ProductController(IUnitOfWork unitOfWork, IFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //src.Adapt(dest)
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var products = await unitOfWork.Products.GetAllWithCategoryAsync();

            TypeAdapterConfig<Product, ProductListDTO>
                .NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);

            var productDto = products.Adapt<List<ProductListDTO>>();

            return Json(new { data = productDto });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await unitOfWork.Categories.GetAllAsync();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    productVM.Product.Img = fileService.UploadFile(file, "Products");
                }

                await unitOfWork.Products.AddAsync(productVM.Product);
                await unitOfWork.CompleteAsync();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categories = await unitOfWork.Categories.GetAllAsync();

            ProductVM productVM = new ProductVM()
            {
                Product = await unitOfWork.Products.GetByIdAsync(id.Value),
                CategoryList = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }
        
        [HttpPost]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    productVM.Product.Img = fileService.UploadFile(file, "Products");
                }

                unitOfWork.Products.UpdateAsync(productVM.Product);
                unitOfWork.CompleteAsync();

                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM.Product);
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var productIndb = await unitOfWork.Products.GetByIdAsync(id.Value);

            if (productIndb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            await unitOfWork.Products.DeleteAsync(productIndb);

            fileService.DeleteFile(productIndb.Img);

            await unitOfWork.CompleteAsync();

            return Json(new { success = true, message = "file has been Deleted" });
        }


    }
}
