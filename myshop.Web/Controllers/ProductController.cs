using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache memoryCache;

        public ProductController(IUnitOfWork unitOfWork, IFileService fileService, IMemoryCache memoryCache)
        {
            this.unitOfWork = unitOfWork;
            this.fileService = fileService;
            this.memoryCache = memoryCache;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //src.Adapt(dest)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await unitOfWork.Products.GetAllWithCategoryAsync();

            TypeAdapterConfig<Product, ProductListDTO>
                .NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);

            var productDto = products.Adapt<List<ProductListDTO>>();

            return View(productDto);
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
            // 1. Validate Image File (Size & Extension)
            if (file != null)
            {
                var validation = fileService.ValidateImage(file);
                if (!validation.IsValid)
                {
                    ModelState.AddModelError("file", validation.ErrorMessage);
                }
            }

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
            // إعادة تحميل القائمة في حال وجود خطأ بالمدخلات
            var categories = await unitOfWork.Categories.GetAllAsync();
            productVM.CategoryList = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

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
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            // 1. Validate Image File if uploaded
            if (file != null)
            {
                var validation = fileService.ValidateImage(file);
                if (!validation.IsValid)
                {
                    ModelState.AddModelError("file", validation.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // حذف الصورة القديمة لمنع وجود Orphan Files
                    if (!string.IsNullOrEmpty(productVM.Product.Img))
                    {
                        fileService.DeleteFile(productVM.Product.Img);
                    }

                    productVM.Product.Img = fileService.UploadFile(file, "Products");
                }

                await unitOfWork.Products.UpdateAsync(productVM.Product);
                await unitOfWork.CompleteAsync();

                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            // إعادة تحميل القائمة في حال وجود خطأ بالمدخلات
            var categories = await unitOfWork.Categories.GetAllAsync();
            productVM.CategoryList = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return View(productVM.Product);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return Json(new { success = false, message = "Invalid ID" });

            var productIndb = await unitOfWork.Products.GetByIdAsync(id.Value);

            if (productIndb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            // 1. حذف الصورة الفيزيائية أولاً
            if (!string.IsNullOrEmpty(productIndb.Img))
            {
                fileService.DeleteFile(productIndb.Img);
            }
            await unitOfWork.Products.DeleteAsync(productIndb);
            await unitOfWork.CompleteAsync();
            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("index");
        }
        // عرض المنتجات المؤرشفة
        [HttpGet]
        public async Task<IActionResult> ArchivedProducts()
        {
            var archivedProducts = await unitOfWork.Products.GetArchivedAsync();
            return View(archivedProducts);
        }

        // إجراء الاستعادة عند الضغط على زر Restore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var product = await unitOfWork.Products.GetByIdWithDeletedAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await unitOfWork.Products.RestoreAsync(product);
            await unitOfWork.CompleteAsync(); // حفظ التغييرات في قاعدة البيانات

            // تفريغ الكاش لتظهر المنتجات المستعادة في شاشات العرض فوراً
            memoryCache.Remove("Categories_List");

            TempData["Success"] = "Product restored successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
