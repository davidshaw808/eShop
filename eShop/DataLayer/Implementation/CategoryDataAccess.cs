using Common.Models.Mutable;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Databases.Base.NonDomainEntites;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace DataLayer.Implementation;

public class CategoryDataAccess : ICategoryDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext, Guid, Task<int?>> _categoryIdGetter;
    private readonly Func<eShopBaseContext, Guid, IAsyncEnumerable<Tuple<int?, Guid?>>> _categoriesProductKeysGetter;
    private readonly Func<eShopBaseContext, int, IAsyncEnumerable<int>> _categoryProductKeysGetter;
    private readonly Func<eShopBaseContext, Guid, Task<Category?>> _atomicCategoryGetter;
    private readonly Func<eShopBaseContext, Guid, Task<Category?>> _uowCategoryGetter;

    public CategoryDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
    {
        _db = unitOfWork.GetContext();
        _categoryIdGetter = GetCompiledCategoryId();
        _categoriesProductKeysGetter = GetCompiledCategoriesProductKeys();
        _categoryProductKeysGetter = GetCompiledCategoryProductKeys();
        _atomicCategoryGetter = GetAtomicCompiledCategory();
        _uowCategoryGetter = GetUoWCompiledCategory();
    }

    public async Task AddChildAtomicAsync(Category parent, Category child)
    {
        if (!parent.Key.HasValue)
            return;

        child.Id = null;
        //if child does not already exist generate key, otherwise get it's pk from the database
        if (!child.Key.HasValue)
            child.GenerateForDatabase();
        else
        {
            var childId = await _categoryIdGetter(_db, child.Key.Value);
            if (!childId.HasValue)
                return;
            child.Id = childId;
        }

        await _db.Categories.SingleMergeAsync(child, options =>
        {
            options.ColumnPrimaryKeyExpression = c => c.Key;
            options.IgnoreOnUpdateExpression = c => c.Id;
            options.AllowUpdatePrimaryKeys = false;
            options.IncludeGraph = true;
            options.IncludeGraphOperationBuilder = operation =>
            {
                if (operation is BulkOperation<Category>)
                {
                    var bulk = (BulkOperation<Category>)operation;
                    bulk.ColumnPrimaryKeyExpression = x => x.Key;
                }
            };
        });
    }

    public async Task LogicalDeleteElementInUow(Category category)
    {
        if (!category.Key.HasValue)
            return;

        if (!_db.ExistsLocally(category))
        {
            var categoryId = await _categoryIdGetter(_db, category.Key.Value);
            if (!categoryId.HasValue)
                return;
            //set pk
            category.Id = categoryId.Value;
        }
        _db.LogicalDelete(category);
    }

    public async Task<bool> LogicalDeleteAtomicAsync(Category category)
    {
        if (!category.Key.HasValue)
            return false;

        var affected = await _db.Categories
            .Where(a => a.Key.Equals(category.Key.Value))
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
        return affected > 0;
    }

    public async Task UpdateAtomicAsync(IEnumerable<Category> categories)
    {
        await _db.Categories.BulkMergeAsync(
            categories,
            options => {
                options.ColumnPrimaryKeyExpression = c => c.Key;
                options.IncludeGraph = false;
                options.IgnoreOnUpdateExpression = c => new { c.Id };
                options.InsertIfNotExists = true;
            });
    }

    public async Task UpdateElementInUoW(Category category)
    {
        if (!_db.ExistsLocally(category))
        {
            if (!await UpdateCategoryWithIds(category))
                return;
        }
        //update everything, alternative is to pull back the entity
        //from the db and figure out what has changed and only update
        //those bits but so far we've only done a light load of id's
        //for ef core's tracking so this is not terrible
        var productKeys = category.Products?.Where(p => p.Id > 0).Select(p => p.Id.Value)?.ToArray();
        var currentProductIds = _categoryProductKeysGetter(_db, category.Id.Value).ToBlockingEnumerable();
        var keysToRemove = productKeys?.Except(currentProductIds).Select(id => new CategoryProduct { CategoryId = category.Id.Value, ProductId = id }) ?? [];
        _db.CategoryProducts.RemoveRange(keysToRemove);
        _db.Categories.Update(category);
    }

    public async Task<bool> UpdateAtomicAsync(Category category)
    {
        //note to self, ef core does not allow multiple concurrent
        //threads, also async updates are really only for leaf properties
        //it can't handle anything graph related easily.
        //This is probably the quickest way unless you generate multiple contexts and
        //share a transaction between them all - though the set up for that will be more intense
        //than this way

        var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            //unlink all the assiciated producets from the category that were not specified in the incoming category
            var productKeys = category.Products?.Select(p => p.Key)?.ToArray();
            if(productKeys?.Length > 0) 
            { 
                await _db.CategoryProducts
                    .Where(cp => cp.Category.Key.Equals(category.Key) && !productKeys.Contains(cp.Product.Key))
                    .ExecuteDeleteAsync();
            }

            await _db.Categories.SingleMergeAsync(category, options =>
            {
                options.ColumnPrimaryKeyExpression = c => c.Key;
                options.IgnoreOnUpdateExpression = c => c.Id;
                options.AllowUpdatePrimaryKeys = false;
                options.IncludeGraph = true;
                options.IncludeGraphOperationBuilder = operation =>
                {
                    if (operation is BulkOperation<Category>)
                    {
                        var bulk = (BulkOperation<Category>)operation;
                        bulk.ColumnPrimaryKeyExpression = x => x.Key;
                    }
                    else if (operation is BulkOperation<Product>)
                    {
                        var bulk = (BulkOperation<Product>)operation;
                        bulk.ColumnPrimaryKeyExpression = x => x.Key;
                    }
                };
            });
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex) 
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public void GenerateElementInUoW(Category category)
    {
        category.GenerateForDatabase();
        _db.Categories.Add(category);
    }

    public async Task GenerateAtomicAsync(Category category)
    {
        category.GenerateForDatabase();
        await _db.Categories.SingleInsertAsync(category);
    }

    public async Task<Category?> GetAtomicAsync(Guid key) => await _atomicCategoryGetter(_db, key);
    
    public async Task<Category?> GetElementInUoW(Guid key) => await _uowCategoryGetter(_db, key);

    private async Task<bool> UpdateCategoryWithIds(Category category)
    {
        IAsyncEnumerable<Tuple<int?, Guid?>> productKeys;
        //get the list of product keys for the category from the database
        if (category.Key.HasValue)
            productKeys = _categoriesProductKeysGetter(_db, category.Key.Value);
        else
            return false;

        if (category.Parent?.Key.HasValue ?? false)
            category.Parent.Id = await _categoryIdGetter(_db, category.Parent.Key.Value);

        await foreach (var p in productKeys)
        {
            if (p.Item2 is not null)
            {
                var prod = category.Products.FirstOrDefault(t => t.Key.Equals(p.Item2));
                if (prod is not null)
                    prod.Id = p.Item1;
            }
        }
        
        return true;
    }

    private Func<eShopBaseContext, Guid, Task<int?>> GetCompiledCategoryId() => EF.CompileAsyncQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Where(a => a.Key.Equals(Key))
        .Select(a => a.Id)
        .FirstOrDefault()
    );

    private Func<eShopBaseContext, Guid, IAsyncEnumerable<Tuple<int?, Guid?>>> GetCompiledCategoriesProductKeys() => EF.CompileAsyncQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Where(c => c.Key.Equals(Key))
        .SelectMany(c => c.Products.Select(p => new Tuple<int?, Guid?>(p.Id, p.Key)))
    );

    private Func<eShopBaseContext, int, IAsyncEnumerable<int>> GetCompiledCategoryProductKeys() => EF.CompileAsyncQuery(
    (eShopBaseContext db, int id) => db.CategoryProducts
        .Where(cp => cp.CategoryId.Equals(id))
        .Select(cp => cp.ProductId)
    );

    private Func<eShopBaseContext, Guid, Task<Category?>> GetAtomicCompiledCategory() => EF.CompileAsyncQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .AsNoTracking()
        .Include(c => c.Products)
        .Include(c => c.Parent)
        .Include(c => c.Children)
        .FirstOrDefault(c => c.Key.Equals(Key))
    );

    private Func<eShopBaseContext, Guid, Task<Category?>> GetUoWCompiledCategory() => EF.CompileAsyncQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .AsNoTracking()
        .Include(c => c.Products)
        .Include(c => c.Parent)
        .Include(c => c.Children)
        .FirstOrDefault(c => c.Key.Equals(Key))
    );
}
