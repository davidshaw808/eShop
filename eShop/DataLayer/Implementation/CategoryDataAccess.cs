using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class CategoryDataAccess : ICategoryDataAccess
    {
        private readonly eShopBaseContext _db;

        public CategoryDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool AddChild(Category p, Category c)
        {
            var par = this._db.Categories.FirstOrDefault(t => t.Id.Equals(p.Id));
            if (par == null)
            {
               return false;
            }
            if (par.Children == null)
            {
                par.Children = new List<Common.Category>();
            }
            par.Children.Add(c);
            this._db.SaveChanges();
            return true;
        }

        public bool Delete(Category t)
        {
            this._db.Categories.Remove(t);
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(Category t)
        {
            if (t.Id != null)
            {
                return false;
            }
            this._db.Categories.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public Category? Get(int id)
        {
            return this._db.Categories.FirstOrDefault(ca => ca.Id.Equals(id));
        }

        public bool Update(Category t)
        {
            /*var c = this._db.Categories.FirstOrDefault(ca => ca.Id.Equals(t.Id));
            if (c != null)
            {
                return false;
            }
            c.Active = t.Active;
            c.Children = t.Children;
            c.Name = t.Name;
            c.Parent = t.Parent;
            c.Products = t.Products;*/
            this._db.Categories.Update(t);
            this._db.SaveChanges();
            return true;
        }

        public bool Update(IEnumerable<Category> t)
        {
            this._db.Categories.UpdateRange(t);
            this._db.SaveChanges();
            return true;
        }
    }
}
