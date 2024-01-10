using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Toolbox
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets the table name from the <see cref="=DbContext"/> for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContext">The DbContext</param>
        /// <returns>The table name for the entity.</returns>
        /// <exception cref="InvalidOperationException">A type was passed that is not an entity in the DbContext.</exception>
        public static string GetTableName<TEntity>(this DbContext dbContext)
            where TEntity : class
            => dbContext.Set<TEntity>().EntityType.GetSchemaQualifiedTableName() ?? throw new InvalidOperationException($"Could not find table name for {typeof(TEntity).FullName} in the DbContext.");

        public static Task<int> AddBatchAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> values) where TEntity : class
            => dbContext.AddRangeAsync(values).ContinueWith(t => dbContext.SaveChangesAsync()).Unwrap();
    }
}
