using BusinessLayer.Interface.User;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User
{
    public class ProductService(IReviewDataAccess reviewDataAccess) : BaseProductService, IProductService
    {
        private readonly IReviewDataAccess _reviewDataAccess = reviewDataAccess;

        public bool Delete(Review review, string userEmail)
        {
            if(review.Owner.Equals(userEmail))
            { 
                return this._reviewDataAccess.Delete(review);
            }
            return false;
        }

        public bool Generate(Review review)
        {
            if(review.Product != null && review.Owner != null)
            {
                return this._reviewDataAccess.Generate(review);
            }
            return false;
        }

        public bool Update(Review review, string userEmail)
        {
            if (review.Owner.Equals(userEmail))
            {
                return this._reviewDataAccess.Update(review);
            }
            return false;
        }

        public IEnumerable<Review> GetAllReviewsForProduct(Product product)
        {
            if(product.Id == null)
            {
                return Enumerable.Empty<Review>();
            }
            return this._reviewDataAccess.GetReviewsForProduct((int)product.Id);
        }
    }
}
