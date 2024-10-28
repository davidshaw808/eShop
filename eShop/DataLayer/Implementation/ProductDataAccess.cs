using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class ProductDataAccess: IProductDataAccess
    {
        private readonly eShopBaseContext _db;

        public ProductDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            this._db = unitOfWork.GetContext();
        }

        public bool LogicalDelete(Product t)
        {
            if(t.Key == null)
            {
                return false;
            }
            var p = this.Get((Guid)t.Key);
            if(p == null)
            {
                return false;
            }
            p.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(Product t)
        {
            if (t.Id != null)
            {
                return false;
            }
            this._db.Products.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public Product? Get(Guid Key)
        {
            return this._db.Products.FirstOrDefault(p => p.Key == Key);
        }

        public bool Update(Product t)
        {
            this._db.Products.Update(t);
            this._db.SaveChanges();
            return true;
        }

        public bool UpdateAll(IEnumerable<Product> products)
        {
            this._db.Products.UpdateRange(products);
            this._db.SaveChanges();
            return true;
        }

        public IEnumerable<Product>? GetAll(IEnumerable<Guid> Keys)
        {
            return this._db.Products.Join(Keys, p => p.Key, aid => aid, (p, aid) =>  p).ToArray();
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledProductId() => EF.CompileQuery(
       (eShopBaseContext db, Guid Key) => db.Products
           .Where(a => a.Key.Equals(Key))
           .Select(a => a.Id)
           .FirstOrDefault()
       );
    }
}
