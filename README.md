# EntityFrameworkCore.Toolbox

Extensions and ease of life code to automattically handle working with Entity Framework and related datasets.

## EfBulkCopy

Easily upload entities to a database, bypassing the database context entirely by automatting the mapping to SqlBulkCopy:

await dbContext.BulkCopy((IEnumerable<TEntity>>) myData);

Or even pull from another source entirely via passing a map of TEntity.Property.Name and a fetch function:

await dbContext.BulkCopy<TEntity, TSource>>((IEnumerable<TSource>>) myData, IDictionary<string, Func<string, object?> entityNameToPropertyGetterMap);