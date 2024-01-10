using Microsoft.EntityFrameworkCore;

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
        public static async Task<bool> TruncateTable<TEntity>(this DbContext dbContext, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var tableName = GetTableName<TEntity>(dbContext);
            if(cancellationToken.IsCancellationRequested) return false;

            try
            {
                return (await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {tableName};", cancellationToken)) == -1 && !cancellationToken.IsCancellationRequested;
            }
            catch (Exception ex)
            {
                if (!ex.Message.EndsWith(" because it is being referenced by a FOREIGN KEY constraint."))
                {
                    throw;
                }
            }

            if (cancellationToken.IsCancellationRequested) return false;

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}; DBCC CHECKIDENT ('{tableName}', RESEED, 0);", cancellationToken);
                return !cancellationToken.IsCancellationRequested;
            }
            catch { }

            return false;
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
