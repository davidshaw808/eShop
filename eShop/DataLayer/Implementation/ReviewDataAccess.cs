using Common;
using Common.Interface;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace DataLayer.Implementation;

public class ReviewDataAccess : IReviewDataAccess
{
    private readonly eShopBaseContext _db;

    public ReviewDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
    {
        _db = unitOfWork.GetContext();
    }

    public async Task LogicalDeleteAsync(Review review)
    {
        var dbReview = await _db.Reviews.FirstOrDefaultAsync(r => r.Key.Equals(review.Key));
        if
        dbReview.Active = false;
    }

    public Task GenerateAsync(Review review)
    {
        review.Id = null;
        return _db.Reviews.AddRange(review);
    }

    public IEnumerable<Review> GetReviewsForProduct(Guid productId)
    {
        return _db.Products.FirstOrDefault(p => p.Key == productId)?.Reviews?.ToArray() ?? Enumerable.Empty<Review>();
    }

    public Task UpdateAsync(Review t)
    {
        throw new NotImplementedException();
    }

    private Func<eShopBaseContext, Guid, int?> GetCompiledReviewId() => EF.CompileQuery(
       (eShopBaseContext db, Guid Key) => db.Reviews
           .Where(a => a.Key.Equals(Key))
           .Select(a => a.Id)
           .FirstOrDefault()
       );
}
