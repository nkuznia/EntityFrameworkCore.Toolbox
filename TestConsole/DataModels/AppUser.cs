using EntityFrameworkCore.Toolbox.Models;
using System.Data;

namespace TestConsole.DataModels
{
    public class AppUser
    {
        public int Id { get; set; }

        [SqlColumn(SqlDbType.VarChar, 100)]
        public string Name { get; set; }

        [SqlColumn(SqlDbType.NVarChar, 400)]
        public string Email { get; set; }

        public UserType Type { get; set; }
    }
}
