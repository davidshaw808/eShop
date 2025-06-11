using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IReviewDataAccess : IUnitOfWorkCRUD<Review>, IAtomicCRUD<Review>
{
    IEnumerable<Review> GetReviewsForProduct(Guid productId);
}

