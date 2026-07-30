using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using myshop.Business.DTOs;
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOperations")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMemoryCache memoryCache;
        private const string CategoryCacheKey = "Categories_List";

        public CategoryController(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
        {
            this.unitOfWork = unitOfWork;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(QueryParameters parameters)
        {
            if (!memoryCache.TryGetValue(CategoryCacheKey, out PagedResult<Category>? categories))
            {
                categories = await unitOfWork.Categories.SearchAsync(parameters.SearchTerm, parameters);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                memoryCache.Set(CategoryCacheKey, categories, cacheOptions);
            }

            categories = await unitOfWork.Categories.SearchAsync(parameters.SearchTerm, parameters);
            
            ViewBag.Parameters = parameters;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            if (ModelState.IsValid)
            {
                await unitOfWork.Categories.AddAsync(category);
                await unitOfWork.CompleteAsync();

                memoryCache.Remove(CategoryCacheKey);

                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            var categoryIndb = await unitOfWork.Categories.GetByIdAsync(id.Value);

            return View(categoryIndb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            if (ModelState.IsValid)
            {
                await unitOfWork.Categories.UpdateAsync(category);
                await unitOfWork.CompleteAsync();

                memoryCache.Remove(CategoryCacheKey);

                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //[HttpGet]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null | id == 0)
        //    {
        //        NotFound();
        //    }
        //    var categoryIndb = await unitOfWork.Categories.GetByIdAsync(id.Value);

        //    return View(categoryIndb);
        //}
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            var categoryIndb = await unitOfWork.Categories.GetByIdAsync(id.Value);
            if (categoryIndb == null)
            {
                NotFound();
            }
            await unitOfWork.Categories.DeleteAsync(categoryIndb);
            await unitOfWork.CompleteAsync();

            memoryCache.Remove(CategoryCacheKey);

            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
