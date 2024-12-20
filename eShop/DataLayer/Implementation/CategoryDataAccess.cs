using Common;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation;

public class CategoryDataAccess : ICategoryDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext, Guid, int?> _categoryIdGetter;
    private readonly Func<eShopBaseContext, Guid, Task<Category?>> _categoryGetter;
    private readonly IDatabaseChangeValidation _dbChangeValidation;

    public CategoryDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork, IDatabaseChangeValidation dbChangeValidation)
    {
        _db = unitOfWork.GetContext();
        _dbChangeValidation = dbChangeValidation;
        _categoryIdGetter = GetCompiledCategoryId();
        _categoryGetter = GetCompiledCategory();
    }

    public async ValueTask<int> AddChildAtomicAsync(Category parent, Category child)
    {
        if (!_dbChangeValidation.Valid(child).Generate)
            return 0;

        if (!parent.Key.HasValue)
            return 0;

        child.Id = null;
        //if child does not already exist generate key, otherwise get it's pk from the database
        if (!child.Key.HasValue)
            child.Generate();
        else
        {
            var childId = _categoryIdGetter(_db, child.Key.Value);
            if (!childId.HasValue)
                return 0;
            child.Id = childId;
        }

        return await _db.Categories
            .Where(c => c.Key.Equals(parent.Key))
            .Include(c => c.Children)
            .ExecuteUpdateAsync(p => p.SetProperty(c => c.Children, c => c.Children.Concat(new [] { child })));
    }

    public void LogicalDeleteElementInUow(Category category)
    {
        if (!_dbChangeValidation.Valid(category).Update)
            return;

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

    public async ValueTask<int> LogicalDeleteAtomicAsync(Category category)
    {
        if (!_dbChangeValidation.Valid(category).Update)
            return 0;

        if (!category.Key.HasValue)
            return 0;

        return await _db.Categories
            .Where(a => a.Key.Equals(category.Key.Value))
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
    }

    public async ValueTask<int> UpdateAtomicAsync(IEnumerable<Category> categories)
    {
        UpdateCategories(categories, true);
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

    public async ValueTask<int> UpdateAtomicAsync(Category category)
    {
        //note to self, ef ccore does not allow multiple concurrent
        //threads, also async updates are really only for leaf properties
        //it can't handle anything graph related easily, the bulkupdate extension can
        //but only works on tracked enumerable so it's two round trips to the db.
        //This is probably the quickest way unless you generate multiple contexts and
        //share a transaction between them all - though the set up for that will be more intense
        //than this way

        if (!category.Key.HasValue ||
            !_dbChangeValidation.Valid(category).Update ||
            !_dbChangeValidation.Valid(category).Generate)
            return 0;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            //do not update the parent and only update immediate children and products
            if (category.Children.Any())
            { 
                await UpdateCategories(category.Children, false);
            }

            if (category.Products.Any())
            {
                category.Products.Where(p => !p.Key.HasValue).Generate();
                await _db.Products.BulkUpdateAsync(
                    category.Products,
                    options => {
                        options.ColumnPrimaryKeyExpression = c => c.Key.Value;
                        options.IncludeGraph = false;
                        options.IgnoreOnUpdateExpression = c => new { c.Id };
                        options.InsertIfNotExists = true;
                    });
            }

            int updated = await _db.Categories
                .Where(c => c.Key.Equals(category.Key))
                .ExecuteUpdateAsync();

            await transaction.CommitAsync();
            return updated;
        }
        catch (Exception ex) 
        { 
            await transaction.RollbackAsync();
            return 0;
        }
    }
    
    public void GenerateElementInUoW(Category category)
    {
        category.Generate();
        _db.Categories.Add(category);
    }

    public Task GenerateAtomicAsync(Category category)
    {
        category.Generate();
        return _db.Categories.SingleInsertAsync(category);
    }

    public Task<Category?> ReadAtomicAsync(Guid key)
    {
        return _categoryGetter(_db, key);
    } 
    
    private async Task UpdateCategories(IEnumerable<Category> categories, bool includeGraph)
    {
        if (!_dbChangeValidation.Valid(categories).Update || !categories.Any())
            return;
        categories.Where(c => !c.Key.HasValue).Generate();

        await _db.Categories.BulkUpdateAsync(
            categories,
            options => {
                options.ColumnPrimaryKeyExpression = c => c.Key.Value;
                options.IncludeGraph = true;
                options.IgnoreOnUpdateExpression = c => new { c.Id };
                options.InsertIfNotExists = true;
            });
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
        .FirstOrDefault(c => c.Key.Equals(Key))
    );

    public void AssignChildenToParent(Category category)
    {
        throw new NotImplementedException();
    }

    ValueTask IAtomicCRUD<Category>.GenerateAtomicAsync(Category t)
    {
        throw new NotImplementedException();
    }

    ValueTask<Category?> IAtomicCRUD<Category>.ReadAtomicAsync(Guid key)
    {
        throw new NotImplementedException();
    }
}
