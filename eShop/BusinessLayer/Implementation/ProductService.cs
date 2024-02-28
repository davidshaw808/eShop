using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class ProductService: ProductOrderService, IProductServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess;

        public ProductService(IProductDataAccess productDataAccess): base(productDataAccess)
        {
            this._productDataAccess = productDataAccess;
        }   

        public bool Delete(Product t)
        {
            if(t.Id == null)
            {
                return false;
            }
            this._productDataAccess.Delete(t);
            return true;
        }

        public bool Generate(Product t)
        {
            return this._productDataAccess.Generate(t);
        }

        public bool IsValidTransaction(IEnumerable<Product> products)
        {
            if(products.Any(p => p.Id == null || p.NumberInStock == null))
            {
                return false;
            }
            var prodList = GetProducsFromOrder(products);
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
