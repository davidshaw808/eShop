using Common;
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
            this._db = unitOfWork.GetContext();
        }

        public int LogicalDelete(Order t, bool commit)
        {
            var delete = this._db.LogicalDelete(t);
            this._db.Orders.;
            return commit && delete ? this._db.SaveChanges() : 0;
        }

        public bool Generate(Order t)
        {
            if (t.Id != null)
            {
                return false;
            }
            t.Key = Guid.NewGuid();
            this._db.Orders.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(PaymentDetails pd)
        {
            if (pd.Id != null)
            {
                return false;
            }
            this._db.PaymentDetails.Add(pd);
            this._db.SaveChanges();
            return true;
        }

        public int Update(PaymentDetails pd, bool commit)
        {
            if(pd.Order == null)
            {
                throw new ArgumentException("Payment does not have an order.");
            }
            if (pd.Id == null)
            {
                this._db.PaymentDetails.Add(pd);
            }
            this._db.PaymentDetails.Update(pd);
            return commit ? this._db.SaveChanges() : 0;
        }

        public Order? Get(Guid Key)
        {
            return this._db.Orders.FirstOrDefault(o => o.Key == Key);
        }

        public IEnumerable<Order> Get(Func<Order, bool> filter)
        {
            return this._db.Orders.Where(filter);
        }

        public int Update(Order t, bool commit)
        {
            this._db.Orders.Update(t);
            this._db.SaveChanges();
        }

        public bool Update(IEnumerable<Order> t)
        {
            this._db.Orders.UpdateRange(t);
            this._db.SaveChanges();
            return true;
        }

        public Task<Order?> GetAsync(Guid Key)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Order> GetAsync(Func<Order, bool> filter)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Order> GetOrdersAsync(Guid CustKey)
        {
            throw new NotImplementedException();
        }

        public bool Update(PaymentDetails paymentDetails)
        {
            throw new NotImplementedException();
        }

        public Task<int> GenerateAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public Task<int> LogicalDeleteAsync(Order t)
        {
            throw new NotImplementedException();
        }

        public int Generate(Order t, bool commit)
        {
            throw new NotImplementedException();
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledOrderId() => EF.CompileQuery(
       (eShopBaseContext db, Guid Key) => db.Orders
           .Where(a => a.Key.Equals(Key))
           .Select(a => a.Id)
           .FirstOrDefault()
       );
    }
}
