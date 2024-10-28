using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IReviewDataAccess : IUnitOfWorkCUD<Review>, IAtomicCRUD<Review>
{
    IEnumerable<Review> GetReviewsForProduct(Guid productId);
}

