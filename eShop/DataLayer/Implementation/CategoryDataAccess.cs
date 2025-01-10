using Common;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.CompilerServices;
using Z.BulkOperations;

namespace DataLayer.Implementation;

public class CategoryDataAccess : ICategoryDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext, Guid, int?> _categoryIdGetter;
    private readonly Func<eShopBaseContext, Guid, IEnumerable<Tuple<int?, Guid?>>> _categoryProdutKeysGetter;
    private readonly Func<eShopBaseContext, Guid, Task<Category?>> _categoryGetter;
    private readonly IDatabaseChangeValidation _dbChangeValidation;

    public CategoryDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork, IDatabaseChangeValidation dbChangeValidation)
    {
        _db = unitOfWork.GetContext();
        _dbChangeValidation = dbChangeValidation;
        _categoryIdGetter = GetCompiledCategoryId();
        _categoryProdutKeysGetter = GetCompiledCategoriesProductKeys();
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
            child.GenerateForDatabase();
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
            .ExecuteUpdateAsync(p => p.SetProperty(c => c.Children, c => c.Children.Concat(new[] { child })));
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
        if (!_db.ExistsLocally(category))
        {
            if (!UpdateCategoryWithIds(category))
                return;
        }
        //update everything, alternative is to pull back the entity
        //from the db and figure out what has changed and only update
        //those bits but so far we've only done a light load of id's
        //for ef core's tracking so this is not terrible
        var productKeys = category.Products?.Select(p => p.Id)?.ToArray();
        if (productKeys?.Length > 0)
        {
            _db.CategoryProducts
                .Where(cp => cp.Category.Key.Equals(category.Key) && !productKeys.Contains(cp.Product.Key))
                .dele();
        }
        _db.Categories.Update(category);
    }

    public async ValueTask UpdateAtomicAsync(Category category)
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
            // await _db.Database.ExecuteSqlAsync($"delete old property references here using _db.OrderProductsTableName")
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
        }
        catch (Exception ex) 
        {
            await transaction.RollbackAsync();
        }
    }

    public void GenerateElementInUoW(Category category)
    {
        category.GenerateForDatabase();
        _db.Categories.Add(category);
    }

    public Task GenerateAtomicAsync(Category category)
    {
        category.GenerateForDatabase();
        return _db.Categories.SingleInsertAsync(category);
    }

    public Task<Category?> ReadAtomicAsync(Guid key)
    {
        return _categoryGetter(_db, key);
    }

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

    private async Task UpdateCategories(IEnumerable<Category> categories, bool includeGraph)
    {
        if (!_dbChangeValidation.Valid(categories).Update || !categories.Any())
            return;
        categories.Where(c => !c.Key.HasValue).GenerateForDatabase();

        await _db.Categories.BulkUpdateAsync(
            categories,
            options => {
                options.ColumnPrimaryKeyExpression = c => c.Key.Value;
                options.IncludeGraph = true;
                options.IgnoreOnUpdateExpression = c => new { c.Id };
                options.InsertIfNotExists = true;
            });
    }

    private bool UpdateCategoryWithIds(Category category)
    {
        if (!_dbChangeValidation.Valid(category).Update ||
            !_dbChangeValidation.Valid(category).Generate ||
            !_dbChangeValidation.Valid(category.Products).Update ||
            !_dbChangeValidation.Valid(category.Products).Generate)
            return false;

        IEnumerable<Tuple<int?, Guid?>> productKeys = [];
        if (category.Key.HasValue)
            productKeys = _categoryProdutKeysGetter(_db, category.Key.Value);

        if (category.Parent?.Key.HasValue ?? false)
            category.Parent.Id = _categoryIdGetter(_db, category.Parent.Key.Value);

        if (productKeys.Any())
        {
            foreach (var p in category.Products)
                p.Id = productKeys.FirstOrDefault(t => t.Item2.Equals(p.Key))?.Item1;
        }
        return true;
    }

    private Func<eShopBaseContext, Guid, int?> GetCompiledCategoryId() => EF.CompileQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Where(a => a.Key.Equals(Key))
        .Select(a => a.Id)
        .FirstOrDefault()
    );

    private Func<eShopBaseContext, Guid, IEnumerable<Tuple<int?, Guid?>>> GetCompiledCategoriesProductKeys() => EF.CompileQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Where(c => c.Key.Equals(Key))
        .SelectMany(c => c.Products.Select(p => new Tuple<int?, Guid?>(p.Id, p.Key)))
    );

    private Func<eShopBaseContext, Guid, IEnumerable<Tuple<int?, Guid?>>> GetCompiledCategoryProductKeys() => EF.CompileQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Where(c => c.Key.Equals(Key))
        .SelectMany(c => c.Products.Select(p => new Tuple<int?, Guid?>(p.Id, p.Key)))
    );

    private Func<eShopBaseContext, Guid, Task<Category?>> GetCompiledCategory() => EF.CompileAsyncQuery(
    (eShopBaseContext db, Guid Key) => db.Categories
        .Include(c => c.Products)
        .Include(c => c.Parent)
        .Include(c => c.Children)
        .FirstOrDefault(c => c.Key.Equals(Key))
    );
}
