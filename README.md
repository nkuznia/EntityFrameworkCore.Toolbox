# EntityFrameworkCore.Toolbox

Extensions and ease of life code to automattically handle working with Entity Framework and related datasets.

Tested and working with Entity Framework Core Sql Server. Other backends may require additional work. Open to pull requests.

## EfBulkCopy

Easily upload entities to a database, bypassing the database context entirely by automatting the mapping to SqlBulkCopy:

```cs
await dbContext.BulkCopyAsync(myData);
```

Or even pull from another source entirely via passing a map of TEntity.Property.Name and a fetch function:

```cs
await dbContext.BulkCopyAsync<TEntity, TSource>>(myData, entityNameToPropertyGetterMap);
```

## Enum reference helpers

Add reference tables to the database automatically or as needed.

```cs
public class MyDbContext : DbContext
{
...
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.AddEnumReferenceTable<MyEnum>(); // Add enums one at a time

		modelBuilder.AddEnumReferenceTables(); // Add all enums in the assembly
	}
...
}
```