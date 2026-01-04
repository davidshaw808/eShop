using BusinessLayer.Interface.Admin;
using Common.Models.Mutable;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.AdminServices;

public class CategoryServiceAdmin : ICategoryServiceAdmin
{
    readonly ICategoryDataAccess _categoryDataAccess;
    readonly IProductServiceAdmin _productService;

    public CategoryServiceAdmin(ICategoryDataAccess categoryDataAccess, IProductServiceAdmin productService)
    {
        _categoryDataAccess = categoryDataAccess;
        _productService = productService;
    }

    public bool LogicalDelete(Category t)
    {
        if (t.Id == null)
        {
            return false;
        }
        var c = _categoryDataAccess.Get((int)t.Id);
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
        _categoryDataAccess.Update(c.Children);
        _categoryDataAccess.LogicalDelete(c);
        return true;
    }

    public bool Generate(Category t)
    {

        if (t.Id != null) { return false; }
        _categoryDataAccess.Generate(t);
        return true;
    }

    public bool Update(Category t)
    {
        var newProducts = t.Products?.Where(p => p.Id == null);
        if (newProducts.Any())
        {
            foreach (var p in newProducts)
            {
                _productService.Generate(p);
            }
        }
        if (t.Id == null)
        {
            return false;
        }
        return _categoryDataAccess.Update(t);
    }
}
