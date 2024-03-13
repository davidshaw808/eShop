using Common.Interface;
using Common;


namespace BusinessLayer.Interface.User
{
    public interface IProductService 
    {
        bool Generate(Review review);
        bool Update(Review review, string userEmail);
        bool Delete(Review review, string userEmail);
        IEnumerable<Review> GetAllReviewsForProduct(Product product);
    }
}
