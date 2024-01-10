using Microsoft.EntityFrameworkCore;
namespace EntityFrameworkCore.Toolbox.Bulk
{
    public static class EfBulkCopyExtensions
    {
        /// <summary>
        /// Create a new instance of EfBulkCopy, write entities to server, then dispose EfBulkCopy.
        /// </summary>
        /// <typeparam name="TEntity">The entity table to write to.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="values">The values to bulkcopy to the server</param>
        public static async Task BulkCopy<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values)
            where TEntity : class
        {
            using var bulkCopy = await EfBulkCopy<TEntity>.CreateAsync(dbContext);
            await bulkCopy.WriteToServerAsync(values);
        }

        /// <summary>
        /// Create a new instance of EfBulkCopy, write from source to server, then dispose EfBulkCopy.
        /// </summary>
        /// <typeparam name="TEntity">The entity table to write to.</typeparam>
        /// <typeparam name="TSource">The IEnumerable source data type.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="values">The values to bulkcopy to the server</param>
        public static async Task BulkCopy<TEntity, TSource>(this DbContext dbContext, IEnumerable<TSource> values, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap)
            where TEntity : class
            where TSource : class
        {
            using var bulkCopy = await EfBulkCopy<TEntity, TSource>.CreateAsync(dbContext, entityParamNameToSourceMap);
            await bulkCopy.WriteToServerAsync(values);
        }
    }
}
