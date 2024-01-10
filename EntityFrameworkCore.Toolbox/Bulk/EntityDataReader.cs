using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EntityFrameworkCore.Toolbox.Bulk
{
    internal class EntityDataReader<TEntity> : IDataReader where TEntity : class
    {
        private Dictionary<int, Func<TEntity, object?>> _map;
        private IEnumerator<TEntity> _enumerator;

        public EntityDataReader(Dictionary<int, Func<TEntity, object?>> map, IEnumerable<TEntity> entities)
        {
            this._map = map;
            this._enumerator = entities.GetEnumerator();
        }

        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => _map.Count;

        public void Close() => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();

        public bool GetBoolean(int i) => throw new NotImplementedException();

        public byte GetByte(int i) => throw new NotImplementedException();

        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();

        public char GetChar(int i) => throw new NotImplementedException();

        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => throw new NotImplementedException();

        public IDataReader GetData(int i) => throw new NotImplementedException();

        public string GetDataTypeName(int i) => throw new NotImplementedException();

        public DateTime GetDateTime(int i) => throw new NotImplementedException();

        public decimal GetDecimal(int i) => throw new NotImplementedException();

        public double GetDouble(int i) => throw new NotImplementedException();

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
        public Type GetFieldType(int i) => throw new NotImplementedException();

        public float GetFloat(int i) => throw new NotImplementedException();

        public Guid GetGuid(int i) => throw new NotImplementedException();

        public short GetInt16(int i) => throw new NotImplementedException();

        public int GetInt32(int i) => throw new NotImplementedException();

        public long GetInt64(int i) => throw new NotImplementedException();

        public string GetName(int i) => throw new NotImplementedException();

        public int GetOrdinal(string name) => throw new NotImplementedException();

        public DataTable? GetSchemaTable() => throw new NotImplementedException();

        public string GetString(int i) => throw new NotImplementedException();

        public object GetValue(int i) => _map[i](_enumerator.Current)!;

        public int GetValues(object[] values) => throw new NotImplementedException();

        public bool IsDBNull(int i) => throw new NotImplementedException();

        public bool NextResult() => throw new NotImplementedException();

        public bool Read() => _enumerator.MoveNext();
    }
}