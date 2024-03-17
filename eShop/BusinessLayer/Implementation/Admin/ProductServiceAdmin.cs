using BusinessLayer.ClassHelpers.Extensions;
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

        public bool LogicalDelete(Product t)
        {
            if (t.Id == null)
            {
                return false;
            }
            _productDataAccess.LogicalDelete(t);
            return true;
        }

        public bool Generate(Product t)
        {
            t.AltId = Guid.NewGuid();
            return _productDataAccess.Generate(t);
        }

        public bool Update(Product t)
        {
            throw new NotImplementedException();
        }
    }
}
