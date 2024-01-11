using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Collections;
using System.Collections.Concurrent;

namespace EntityFrameworkCore.Toolbox.Bulk
{
    public class EfBulkCopy<TEntity> : EfBulkCopy<TEntity, TEntity> where TEntity : class
    {
        protected EfBulkCopy(DbContext dbContext, Dictionary<int, Func<TEntity, object?>> map)
            : base(dbContext, map)
        {

        }

        public static async Task<EfBulkCopy<TEntity>> CreateAsync(DbContext dbContext, CancellationToken cancellationToken = default)
        {
            var entityParamNameToSourceMap = dbContext.Set<TEntity>().EntityType.GetProperties()
                .ToDictionary(p => p.Name, p => (Func<TEntity, object?>)((TEntity e) => e == null ? default : p?.PropertyInfo?.GetValue(e) ?? default), StringComparer.Ordinal);

            var mapTask = GetColumnMapAsync(dbContext, entityParamNameToSourceMap, cancellationToken);

            return new EfBulkCopy<TEntity>(dbContext, await mapTask);
        }
    }

    public class EfBulkCopy<TEntity, TSource> : IDisposable
        where TEntity : class
        where TSource : class
    {
        protected SqlBulkCopy _bulkCopy;
        protected Dictionary<int, Func<TSource, object?>> _map;
        protected bool _isDisposed;
        protected static readonly ConcurrentDictionary<Tuple<Type, Type, Type>, IDictionary> _typeMap = new ConcurrentDictionary<Tuple<Type, Type, Type>, IDictionary>();

        protected EfBulkCopy(DbContext dbContext, Dictionary<int, Func<TSource, object?>> map)
        {
            var connectionString = dbContext.Database.GetConnectionString();
            var tableName = dbContext.GetTableName<TEntity>();
            var bulkCopy = new SqlBulkCopy(connectionString) { DestinationTableName = tableName };

            _bulkCopy = bulkCopy;
            _map = map;
        }

        public static async Task<EfBulkCopy<TEntity, TSource>> CreateAsync(DbContext dbContext, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap, CancellationToken cancellationToken = default)
            => new EfBulkCopy<TEntity, TSource>(dbContext, await GetColumnMapAsync(dbContext, entityParamNameToSourceMap, cancellationToken));

        public Task WriteToServerAsync(IEnumerable<TSource> data, CancellationToken cancellationToken = default)
            => _bulkCopy.WriteToServerAsync(new EntityDataReader<TSource>(_map, data), cancellationToken);


        protected static async Task<Dictionary<int, Func<TSource, object?>>> GetColumnMapAsync(DbContext dbContext, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap, CancellationToken cancellationToken = default)
        {
            if (TryGetTypeMap(dbContext, out var cachedDict))
            {
                return cachedDict as Dictionary<int, Func<TSource, object?>> ?? throw new Exception("Bad cached dictionary mapping was unable to parse");
            }

            return await Task.Run(() => BuildColumnMap(dbContext, entityParamNameToSourceMap), cancellationToken);

            static Dictionary<int, Func<TSource, object?>> BuildColumnMap(DbContext dbContext, IDictionary<string, Func<TSource, object?>> entityParamNameToSourceMap)
            {
                var datatable = new DataTable();
                using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM " + dbContext.GetTableName<TEntity>(), dbContext.Database.GetConnectionString()))
                {
                    da.Fill(datatable);
                }

                var entityProperties = dbContext.Set<TEntity>().EntityType.GetProperties();
                var dict = new Dictionary<int, Func<TSource, object?>>();

                foreach (DataColumn column in datatable.Columns)
                {
                    var property = entityProperties.FirstOrDefault(e => column.ColumnName.Equals(e.GetColumnName()));
                    if (property == null) continue;
                    Func<TSource, object?> propGet = entityParamNameToSourceMap.TryGetValue(property.Name, out var func) ? func : throw new Exception($"Property {property.ClrType.FullName}.{property.Name} not in provided mapping dictionary for EfBulkCopy.");

                    dict[column.Ordinal] = propGet;
                }

                TrySetTypeMap(dbContext, dict);
                return dict;
            }
        }

        protected static bool TryGetTypeMap(DbContext context, out IDictionary<int, Func<TSource, object?>> maps)
        {
            if (_typeMap.TryGetValue(Tuple.Create(context.GetType(), typeof(TEntity), typeof(TSource)), out var cachedMap) && cachedMap is IDictionary<int, Func<TSource, object?>> mapResult)
            {
                maps = mapResult;
                return true;
            }

            maps = null!;
            return false;
        }

        protected static bool TrySetTypeMap(DbContext context, IDictionary maps)
            => _typeMap.TryAdd(Tuple.Create(context.GetType(), typeof(TEntity), typeof(TSource)), maps);

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    (_bulkCopy as IDisposable)?.Dispose();
                    _map = null!;
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EfBulkCopy()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
