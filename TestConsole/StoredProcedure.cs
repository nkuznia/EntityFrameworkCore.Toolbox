using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Toolbox.Models
{
    public class StoredProcedure
    {
        public string Name { get; set; }
        public SqlParameterCollection Parameters { get; set; }
    }

    public class StoredProcedure<T> : StoredProcedure
    {
        

    }
}
