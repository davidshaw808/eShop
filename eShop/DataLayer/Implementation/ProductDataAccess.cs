using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class ProductDataAccess: IProductDataAccess
    {
        private readonly eShopBaseContext _db;

        public ProductDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool LogicalDelete(Product t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            var p = this.Get((Guid)t.AltId);
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

        public Product? Get(Guid altId)
        {
            return this._db.Products.FirstOrDefault(p => p.AltId == altId);
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

        public IEnumerable<Product>? GetAll(IEnumerable<Guid> AltIds)
        {
            return this._db.Products.Join(AltIds, p => p.AltId, aid => aid, (p, aid) =>  p).ToArray();
        }
    }
}
