using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin
{
    public class ProductServiceAdmin : IProductServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess;

        public ProductServiceAdmin(IProductDataAccess productDataAccess)
        {
            _productDataAccess = productDataAccess;
        }

        public bool Delete(Product t)
        {
            if (t.Id == null)
            {
                return false;
            }
            _productDataAccess.Delete(t);
            return true;
        }

        public bool Generate(Product t)
        {
            return _productDataAccess.Generate(t);
        }

        public bool IsValidTransaction(IEnumerable<Product> products)
        {
            if (products.Any(p => p.Id == null || p.NumberInStock == null))
            {
                return false;
            }
            var groupedProds = products
                .GroupBy(p => p.Id)
                .Select(p => new GroupedProduct { Id = (int)p.Key, Products = p.ToList() });

            var prodList = groupedProds
                .Select(gp => new SimpleProduct() { Id = gp.Id, Quantity = gp.Products.Sum(p => p.NumberInStock) })
                .ToArray();

            foreach (var p in prodList)
            {
                if (p.Quantity > 0)
                {
                    p.Quantity--;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public bool Update(Product t)
        {
            throw new NotImplementedException();
        }
    }
}
