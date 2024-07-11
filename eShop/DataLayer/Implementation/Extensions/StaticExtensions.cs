using Common.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation.Extensions
{
    internal static class StaticExtensions
    {
        public static bool ExistsLocally<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e.Equals(entity));
        }

        /// <summary>
        /// Ensures you are tracking an existing entity's changes from point of call until 'SubmitChanges' or 'SumbitChangesAsync' is called
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        public static void Track<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            if (!context.ExistsLocally(entity))
            {
                context.Attach(entity);
            }
        }
    }
}