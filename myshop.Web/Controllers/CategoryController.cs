using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Business.DTOs;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOperations")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
         
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await unitOfWork.Categories.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            if (ModelState.IsValid)
            {
                await unitOfWork.Categories.AddAsync(category);
                await unitOfWork.CompleteAsync();
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
                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            var categoryIndb = await unitOfWork.Categories.GetByIdAsync(id.Value);

            return View(categoryIndb);
        }

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
            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
