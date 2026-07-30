using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess.Common;
using myshop.DataAccess.Entities;
using myshop.DataAccess.Rebositories.Interfaces;


namespace myshop.Web.Components
{
    public class ProductListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductListViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(QueryParameters parameters) 
        {
            var productList = await unitOfWork.Products.SearchAsync(parameters.SearchTerm, parameters);

            return View(productList);
        }
    }
}
