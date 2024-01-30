using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EntityFrameworkCore.Toolbox.Models
{
    public class VarCharColumnAttribute : ColumnAttribute
    {
        public VarCharColumnAttribute() : base()
        {
            SetType(0);
        }

        public VarCharColumnAttribute(string name) : base(name)
        {
            SetType(0);
        }

        public VarCharColumnAttribute(int length = 0)
        {
            SetType(length);
        }

        public VarCharColumnAttribute(string name, int length = 0) : base(name)
        {
            SetType(length);
        }

        private void SetType(int length)
        {
            TypeName = $"{SqlDbType.VarChar}({(length < 1 || length > 8000 ? "MAX" : length.ToString())})";
        }
    }
}
