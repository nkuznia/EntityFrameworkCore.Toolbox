namespace EntityFrameworkCore.Toolbox.Bulk
{
    /*
    public class EfBulkCopy<TEntity> : IDisposable where TEntity : class
    {
        private SqlBulkCopy _bulkCopy;
        private Dictionary<int, Func<TEntity, object?>> _map;
        private bool _isDisposed;
        private static readonly Dictionary<Type, IDictionary> _typeMap = new Dictionary<Type, IDictionary>();

        private EfBulkCopy(SqlBulkCopy bulkCopy, Dictionary<int, Func<TEntity, object?>> map)
        {
            _bulkCopy = bulkCopy;
            _map = map;
        }

        public static async Task<EfBulkCopy<TEntity>> CreateAsync(DbContext dbContext)
        {
            var mapTask = GetColumnMap(dbContext);

            var connectionString = dbContext.Database.GetConnectionString();
            var tableName = dbContext.GetTableName<TEntity>();
            var bulkCopy = new SqlBulkCopy(connectionString) { DestinationTableName = tableName };

            return new EfBulkCopy<TEntity>(bulkCopy, await mapTask);
        }

        public Task WriteToServer(IEnumerable<TEntity> entities) => _bulkCopy.WriteToServerAsync(new EntityDataReader<TEntity>(_map, entities));

        private static async Task<Dictionary<int, Func<TEntity, object?>>> GetColumnMap(DbContext dbContext)
        {
            var type = typeof(TEntity);
            if (_typeMap.TryGetValue(type, out var cachedDict))
            {
                return cachedDict as Dictionary<int, Func<TEntity, object?>> ?? throw new Exception("Bad cached dictionary mapping was unable to parse");
            }

            return await Task.Run(() => BuildColumnMap(dbContext));

            static Dictionary<int, Func<TEntity, object?>> BuildColumnMap(DbContext dbContext)
            {
                var datatable = new DataTable();
                using (var da = new SqlDataAdapter("SELECT TOP 0 * FROM " + dbContext.GetTableName<TEntity>(), dbContext.Database.GetConnectionString()))
                {
                    da.Fill(datatable);
                }

                var entityProperties = dbContext.Set<TEntity>().EntityType.GetProperties();
                var dict = new Dictionary<int, Func<TEntity, object?>>();

                foreach(DataColumn column in datatable.Columns)
                {
                    var property = entityProperties.FirstOrDefault(e => column.ColumnName.Equals(e.GetColumnName()));
                    if (property == null) continue;
                    Func<TEntity, object?> propGet = (TEntity e) => e == null ? default : property?.PropertyInfo?.GetValue(e) ?? default;

                    dict[column.Ordinal] = propGet;
                }

                _typeMap.TryAdd(typeof(TEntity), dict);
                return dict;
            }
        }

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
    */
}
