using BusinessLayer.Interface;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class CategoryService : ICategoryService
    {
        readonly ICategoryDataAccess _categoryDataAccess;
        readonly IProductService _productService;

        public CategoryService(ICategoryDataAccess categoryDataAccess, IProductService productService)
        {
            this._categoryDataAccess = categoryDataAccess;
            this._productService = productService;
        }

        public bool Delete(Category t)
        {
            if(t.Id == null)
            {
                return false;
            }
            var c = this._categoryDataAccess.Get((int)t.Id);
            if (c == null)
            {
                return false;
            }
            if (c.Children != null)
            {
                foreach (var k in c.Children)
                {
                    k.Parent = c.Parent;
                }
            }
            this._categoryDataAccess.Update(c.Children);
            this._categoryDataAccess.Delete(c);
            return true;
        }

        public bool Generate(Category t)
        {

            if(t.Id != null) { return false;  }
            this._categoryDataAccess.Generate(t);
            return true;
        }

        public bool Update(Category t)
        {
            var newProducts = t.Products?.Where(p => p.Id == null);
            if (newProducts.Any())
            {
                foreach(var p in newProducts)
                {
                    this._productService.Generate(p);
                }
            }
            if (t.Id == null)
            {
                return false;
            }
            return this._categoryDataAccess.Update(t);
        }
    }
}
