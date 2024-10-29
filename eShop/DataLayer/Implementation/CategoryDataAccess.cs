using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class CategoryDataAccess : ICategoryDataAccess
    {
        private readonly eShopBaseContext _db;

        public CategoryDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            this._db = unitOfWork.GetContext();
        }

        public Task<int> AddChildAsync(Category parent, Category child)
        {
            return _db.Categories
                .Where(c => c.Key.Equals(parent.Key))
               // .Include(c => c.Children)
                .ExecuteUpdateAsync(p => p.SetProperty(c => c.Children, c => c.Children.Concat(new[] { child })));
        }

        public int LogicalDeleteAsync(Category t, bool commit)
        {
            this._db.LogicalDelete(t);
            if(commit)
            {
                return this._db.SaveChanges();
            }
            return 0;
        }

        public int Generate(Category t, bool commit)
        {
            if (t.Id != null)
            {
                return 0;
            }
            this._db.Categories.Add(t);
            if(commit)
            {
                return this._db.SaveChanges();
            }
            return 0;
        }

        public Task<int> GenerateAsync(Category t)
        {
            if (t.Id != null)
            {
                return Task.FromResult(0);
            }
            this._db.Categories.Add(t);
            return this._db.SaveChangesAsync();
        }

        public Category? Get(int id)
        {
            return this._db.Categories.FirstOrDefault(ca => ca.Id.Equals(id));
        }
        public Task<Category?> GetAsync(int id)
        {
            return this._db.Categories.FirstOrDefaultAsync(ca => ca.Id.Equals(id));
        }

        public int Update(IEnumerable<Category> t, bool commit)
        {
            this._db.Categories.UpdateRange(t);
            if(commit)
            {
                this._db.SaveChanges();
            }
            return 0;            
        }

        /// <summary>
        /// multiple async updates
        /// </summary>
        /// <param name="cats"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(IEnumerable<Category> cats)
        {
            int recordsUpdated = 0;
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 15//15 continuations
            };

            await Parallel.ForEachAsync(
                source: cats,
                parallelOptions: options,
                body: async (cat, ct) =>
                {
                    this._db.Categories.Update(cat);
                    recordsUpdated += await this._db.SaveChangesAsync();
                });
            return recordsUpdated;
        }

        public Task<int> UpdateAsync(Category t)
        {
            this._db.Categories.Update(t);
            return this._db.SaveChangesAsync();
        }

        public int Update(Category t, bool commit)
        {
            this._db.Categories.Update(t);
            return commit ? this._db.SaveChanges() : 0;
        }

        public Task<int> LogicalDeleteAsync(Category t, bool commit)
        {
            this._db.Track(t);
            t.Parent?.AssignParentToChildren();
            t.Active = false;
            return this._db.SaveChangesAsync();
        }

        public Task<int> LogicalDeleteAsync(int id)
        {
            var cat = this._db.Categories.First(c => c.Id.Equals(id) && c.Active);
            cat.Active = false;
            return this._db.SaveChangesAsync();
        }

        public int Update(IEnumerable<Category> cats)
        {
            throw new NotImplementedException();
        }

        public Task<int> LogicalDeleteAsync(Category t)
        {
            throw new NotImplementedException();
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledCategoryId() => EF.CompileQuery(
        (eShopBaseContext db, Guid Key) => db.Categories
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );
    }
}
