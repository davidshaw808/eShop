using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin
{
    public class ProductServiceAdmin(IProductDataAccess productDataAccess,
        IReviewDataAccess reviewDataAccess) : ProductService(reviewDataAccess), IProductServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess = productDataAccess;

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
            var prodList = GroupProducts(products);
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
