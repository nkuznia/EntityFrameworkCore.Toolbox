using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EntityFrameworkCore.Toolbox.Models
{
    public class SqlColumnAttribute : ColumnAttribute
    {
        public SqlColumnAttribute() : base()
        {
        }

        public SqlColumnAttribute(string name) : base(name)
        {
        }

        public SqlColumnAttribute(SqlDbType dbType, int length = 0)
        {
            SetType(dbType, length);
        }

        public SqlColumnAttribute(string name, SqlDbType dbType, int length = 0) : base(name)
        {
            SetType(dbType,length);
        }

        private void SetType(SqlDbType dbType, int length)
        {
            switch (dbType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.NText:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarBinary:
                case SqlDbType.VarChar:
                    TypeName = $"{dbType}({(length < 1 || length > 8000 ? "MAX" : length.ToString())})";
                    return;
                default:
                    TypeName = dbType.ToString();
                    return;
            }
        }
    }
}
