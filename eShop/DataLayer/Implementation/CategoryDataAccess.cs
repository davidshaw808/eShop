using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DataLayer.Implementation
{
    public class CategoryDataAccess : ICategoryDataAccess
    {
        private readonly eShopBaseContext _db;
        private readonly Func<eShopBaseContext, Guid, int?> _categoryIdGetter;
        private readonly Func<eShopBaseContext, Guid, Task<Category?>> _categoryGetter;

        public CategoryDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            _db = unitOfWork.GetContext();
            _categoryIdGetter = GetCompiledCategoryId();
            _categoryGetter = GetCompiledCategory();
        }

        public Task<int> AddChildAtomicAsync(Category parent, Category child)
        {
            if (!parent.Key.HasValue)
                return StaticExtensions.Uncommitted;
            child.Id = null;
            //if child does not already exist generate key, otherwise get it's pk from the database
            if (!child.Key.HasValue)
                child.Key = Guid.NewGuid();
            else
            {
                var childId = _categoryIdGetter(_db, child.Key.Value);
                if (!childId.HasValue)
                    return StaticExtensions.Uncommitted;
                child.Id = childId;
            }

            return _db.Categories
                .Where(c => c.Key.Equals(parent.Key))
                .Include(c => c.Children)
                .ExecuteUpdateAsync(p => p.SetProperty(c => c.Children, c => c.Children.Concat(new [] { child })));
        }

        public void LogicalDeleteElementInUow(Category category)
        {
            if (!category.Key.HasValue)
                return;
            if (!_db.ExistsLocally(category))
            {
                var categoryId = _categoryIdGetter(_db, category.Key.Value);
                if (!categoryId.HasValue)
                    return;
                //set pk
                category.Id = categoryId.Value;
            }
            _db.LogicalDelete(category);
        }

        public Task<int> LogicalDeleteAtomicAsync(Category category)
        {
            if (!category.Key.HasValue)
                return StaticExtensions.Uncommitted;
            return _db.Categories
                .Where(a => a.Key.Equals(category.Key.Value))
                .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
        }

        public Task<int> UpdateAtomicAsync(IEnumerable<Category> categories)
        {
            /* var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 15 };
             Parallel.ForEachAsync(source: categories, parallelOptions: parallelOptions, body: (category) => {

             });*/
            return _db.Categories
                .Join(categories,
                c1 => c1.Key,
                c2 => c2.Key,
                (c1, c2) => new { 
                    OldCategory = c1,
                    CategoryToUpdate = c2 
                })
                .ExecuteUpdateAsync( o => o.SetProperty(p => p.OldCategory, p => p.CategoryToUpdate));
        }

        public void UpdateElementInUoW(Category category)
        {
            if (!category.Key.HasValue)
                return;
            //possible to have multiple updates on the same object, if so and is currently tracked don't bother getting the Id from the db
            if (!_db.ExistsLocally(category))
            {
                var categoryId = _categoryIdGetter(_db, category.Key.Value);
                if (!categoryId.HasValue)
                    return;
                //set pk and start change tracking - as we know it's currently not being tracked
                category.Id = categoryId.Value;
                _db.Attach(category);
            }
            _db.Categories.Update(category);
        }
        public Task<int> UpdateAtomicAsync(Category t)
        {
            throw new NotImplementedException();
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
            t.Parent?.AssignToParent();
            t.Active = false;
            return this._db.SaveChangesAsync();
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledCategoryId() => EF.CompileQuery(
        (eShopBaseContext db, Guid Key) => db.Categories
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

        private Func<eShopBaseContext, Guid, Task<Category?>> GetCompiledCategory() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key) => db.Categories
            .Include(c => c.Products)
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .Where(a => a.Key.Equals(Key))
            .FirstOrDefault()
        );

        public void GenerateElementInUoW(Category t)
        {
            throw new NotImplementedException();
        }

        
        public Task GenerateAtomicAsync(Category t)
        {
            throw new NotImplementedException();
        }

        public Task<Category?> ReadAtomicAsync(Guid key)
        {
            throw new NotImplementedException();
        }      
    }
}
