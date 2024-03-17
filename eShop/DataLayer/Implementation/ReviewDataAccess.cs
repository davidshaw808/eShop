using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class ReviewDataAccess : IReviewDataAccess
    {
        private readonly eShopBaseContext _db;

        public ReviewDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool LogicalDelete(Review review)
        {
            this._db.Reviews.Remove(review);
            return true;
        }

        public bool Generate(Review review)
        {
            this._db.Reviews.Add(review);
            return true;
        }

        public bool Update(Review review)
        {
            this._db.Reviews.Update(review);
            return true;
        }

        public IEnumerable<Review> GetReviewsForProduct(Guid productId)
        {
            return this._db.Products.FirstOrDefault(p => p.AltId == productId)?.Reviews?.ToArray() ?? Enumerable.Empty<Review>();
        }
    }
}
