using Common.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.ClassHelpers.Extensions;

internal static class StaticExtensions
{
    private static bool ContextEquals<TEntity>(this IElement<TEntity> me, IElement<TEntity> other) where TEntity : class => me.Key.Equals(other.Key);

    internal static bool ExistsLocally<TEntity>(this DbContext context, IElement<TEntity> entity) where TEntity : class
    {
        //if an entity is in the db it implements IElement<T>
        return context.Set<TEntity>().Local.Any(e => ((IElement<TEntity>)e).ContextEquals(entity));
    }

    /// <summary>
    /// Ensures you are tracking an existing entity's changes from point of call until 'SubmitChanges' or 'SumbitChangesAsync' is called primary key must be set otherwise it is added
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="context"></param>
    /// <param name="entity"></param>
    internal static void Track<TEntity>(this DbContext context, IElement<TEntity> entity) where TEntity : class
    {
        if (!context.ExistsLocally(entity))
        {
            context.Attach(entity);
        }
    }

    /// <summary>
    /// primary key must be set for logical delete, otherwise entity is added
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="context"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    internal static bool LogicalDelete<TEntity>(this DbContext context, IElement<TEntity> entity) where TEntity : class
    {
        if (entity == null || !entity.Id.HasValue)
        {
            return false;
        }
        context.Track(entity);
        entity.Active = false;
        return true;
    }

    //internal static readonly Task<int> Uncommitted = Task.FromResult(0);
}