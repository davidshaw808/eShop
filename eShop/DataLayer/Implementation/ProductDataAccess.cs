using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class ProductDataAccess: IProductDataAccess
    {
        private readonly CrispHabitatBaseContext _db;

        public ProductDataAccess(CrispHabitatBaseContext db)
        {
            this._db = db;
        }

        public bool Delete(Common.Product t)
        {
            if(t.Id == null)
            {
                return false;
            }
            var p = this.Get((int)t.Id);
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

        public Common.Product? Get(int id)
        {
            return this._db.Products.FirstOrDefault(p => p.Id == id);
        }

        public bool Update(Common.Product t)
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
    }
}
