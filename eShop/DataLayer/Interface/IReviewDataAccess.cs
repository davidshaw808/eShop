using Common.Interface;
using Common;

namespace DataLayer.Interface
{
    public interface IReviewDataAccess : IGenerateUpdateDelete<Review>
    {
        IEnumerable<Review> GetReviewsForProduct(int productId);
    }
}
