using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Toolbox
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets the table name from the <see cref="=DbContext"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContext">The DbContext.</param>
        /// <returns>The table name for the entity.</returns>
        /// <exception cref="InvalidOperationException">A type was passed that is not an entity in the DbContext.</exception>
        public static string GetTableName<TEntity>(this DbContext dbContext)
            where TEntity : class
            => dbContext.Set<TEntity>().EntityType.GetSchemaQualifiedTableName() ?? throw new InvalidOperationException($"Could not find table name for {typeof(TEntity).FullName} in the DbContext.");

        /// <summary>
        /// Truncates table in the <see cref="=DbContext"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContext">The DbContext.</param>
        /// <returns>The bool if successful.</returns>
        /// <exception cref="InvalidOperationException">A type was passed that is not an entity in the DbContext.</exception>
        public static async Task<bool> TruncateTableAsync<TEntity>(this DbContext dbContext, ILogger? logger = default, CancellationToken cancellationToken = default, bool reseed = true)
            where TEntity : class
        {
            if(cancellationToken.IsCancellationRequested) return false;

            var tableName = GetTableName<TEntity>(dbContext);
            var sql = $"BEGIN TRY TRUNCATE TABLE {tableName}; END TRY BEGIN CATCH DELETE FROM {tableName}; {(reseed ? $"DBCC CHECKIDENT('{tableName}', RESEED, 0); " : "")}END CATCH";

            try
            {
                return (await dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken)) == -1 && !cancellationToken.IsCancellationRequested;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Error executing truncate: {sql}");
                return false;
            }
        }


        /// <summary>
        /// A simple helper method to add a range of entities to the DbContext then imemediately save changes.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContext">The DbContext.</param>
        public static Task<int> AddBatchAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values, CancellationToken cancellationToken = default) where TEntity : class
            => dbContext.AddRangeAsync(values, cancellationToken).ContinueWith(t => dbContext.SaveChangesAsync(cancellationToken)).Unwrap();
    }
}
