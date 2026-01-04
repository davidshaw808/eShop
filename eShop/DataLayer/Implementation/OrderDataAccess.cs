using Common;
using Common.Models.Immutable;
using Common.Models.Mutable;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class OrderDataAccess : IOrderDataAccess
    {
        private readonly eShopBaseContext _db;

        public OrderDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            _db = unitOfWork.GetContext();
        }

        public Task GenerateAtomicAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public void GenerateElementInUoW(Order t)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetAtomicAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetElementInUoW(Guid t)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Order> GetOrdersAtomicAsync(Guid CustKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogicalDeleteAtomicAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public Task LogicalDeleteElementInUow(Order t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAtomicAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public Task UpdateElementInUoW(Order t)
        {
            throw new NotImplementedException();
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledOrderId() => EF.CompileQuery(
            (eShopBaseContext db, Guid Key) => db.Orders
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
            );

        private Func<eShopBaseContext, Guid, Order?> GetCompiledOrderNoTrack() => EF.CompileQuery(
            (eShopBaseContext db, Guid Key) => db.Orders
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Payments)
            .Include(o => o.Customer)
            .Include(o => o.PaymentRequests)
            .Include(o => o.Products)
            .Include(o => o.Shipping)
            .Include(o => o.Updates)
            .Where(a => a.Key.Equals(Key))
            .FirstOrDefault()
            );
    }
}
