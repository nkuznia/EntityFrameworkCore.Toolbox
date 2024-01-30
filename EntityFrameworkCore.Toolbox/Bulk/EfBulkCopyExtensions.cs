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
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task BulkCopyAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            using var bulkCopy = await EfBulkCopy<TEntity>.CreateAsync(dbContext, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            await bulkCopy.WriteToServerAsync(values, cancellationToken);
        }

        /// <summary>
        /// Create a new instance of EfBulkCopy, write from source to server, then dispose EfBulkCopy.
        /// </summary>
        /// <typeparam name="TEntity">The entity table to write to.</typeparam>
        /// <typeparam name="TSource">The IEnumerable source data type.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="values">The values to bulkcopy to the server</param>
        /// <param name="entityParamNameToSourceMap">A source to entity property map.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task BulkCopyAsync<TEntity, TSource>(this DbContext dbContext, IEnumerable<TSource> values, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap, CancellationToken cancellationToken = default)
            where TEntity : class
            where TSource : class
        {
            using var bulkCopy = await EfBulkCopy<TEntity, TSource>.CreateAsync(dbContext, entityParamNameToSourceMap, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            await bulkCopy.WriteToServerAsync(values, cancellationToken);
        }

        /// <summary>
        /// Create a new instance of EfBulkCopy with orchestration to pull paged data, write from source to server, then dispose EfBulkCopy.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type.</typeparam>
        /// <typeparam name="TEntity">The entity table to write to.</typeparam>
        /// <typeparam name="TBatch"></typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="batchFetch">A function to create the task to fetch the next batch request.</param>
        /// <param name="isSuccess">A function to determin if the batch request was successful.</param>
        /// <param name="selector">A function to select the TSource items to copy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task BulkCopyAsync<TContext, TEntity, TBatch>(this TContext dbContext, Func<int, TBatch?, CancellationToken?, Task<TBatch>?> batchFetch, Func<TBatch, bool> isSuccess, Func<TBatch, IEnumerable<TEntity>> selector, CancellationToken cancellationToken = default)
            where TContext : DbContext
            where TEntity : class
            where TBatch : class
        {
            var i = 0;
            var batchTask = batchFetch(i++, null, cancellationToken);
            if (batchTask == null || cancellationToken.IsCancellationRequested) return;

            var bulkCreateTask = EfBulkCopy<TEntity>.CreateAsync(dbContext, cancellationToken);

            await Task.WhenAll(batchTask, bulkCreateTask);
            var batch = await batchTask;
            using var bulkCopy = await bulkCreateTask;

            while (batch != null && isSuccess(batch) && selector(batch).Any())
            {
                batchTask = batchFetch(i++, batch, cancellationToken) ?? Task.FromResult<TBatch>(null!);

                await Task.WhenAll(batchTask, bulkCopy.WriteToServerAsync(selector(batch), cancellationToken));
                batch = await batchTask;
            }
        }

        /// <summary>
        /// Create a new instance of EfBulkCopy with orchestration to pull paged data, write from source to server, then dispose EfBulkCopy.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type.</typeparam>
        /// <typeparam name="TEntity">The entity table to write to.</typeparam>
        /// <typeparam name="TSource">The IEnumerable source data type.</typeparam>
        /// <typeparam name="TBatch"></typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <param name="batchFetch">A function to create the task to fetch the next batch request.</param>
        /// <param name="isSuccess">A function to determin if the batch request was successful.</param>
        /// <param name="selector">A function to select the TSource items to copy.</param>
        /// <param name="entityParamNameToSourceMap">A source to entity property map.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task BulkCopyAsync<TContext, TEntity, TSource, TBatch>(this TContext dbContext, Func<int, TBatch?, CancellationToken?, Task<TBatch>?> batchFetch, Func<TBatch, bool> isSuccess, Func<TBatch, IEnumerable<TSource>> selector, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap, CancellationToken cancellationToken = default)
            where TContext : DbContext
            where TEntity : class
            where TSource : class
            where TBatch : class
        {
            var i = 0;
            var batchTask = batchFetch(i++, null, cancellationToken);
            if (batchTask == null || cancellationToken.IsCancellationRequested) return;

            var bulkCreateTask = EfBulkCopy<TEntity, TSource>.CreateAsync(dbContext, entityParamNameToSourceMap, cancellationToken);

            await Task.WhenAll(batchTask, bulkCreateTask);
            var batch = await batchTask;
            using var bulkCopy = await bulkCreateTask;

            while (batch != null && isSuccess(batch) && selector(batch).Any())
            {
                batchTask = batchFetch(i++, batch, cancellationToken) ?? Task.FromResult<TBatch>(null!);

                await Task.WhenAll(batchTask, bulkCopy.WriteToServerAsync(selector(batch), cancellationToken));
                batch = await batchTask;
            }
        }
    }
}
