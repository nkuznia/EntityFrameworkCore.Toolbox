using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EntityFrameworkCore.Toolbox.Models
{
    /// <summary>
    /// A model used to create enum reference tables through EF.
    /// </summary>
    public class EnumReference
    {
        /// <summary>
        /// Gets or sets the arbitrary database identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the Enum value.
        /// </summary>
        [Column(TypeName = nameof(SqlDbType.VarChar))]
        [MaxLength(400)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the enum.
        /// </summary>
        public long Value { get; set; }
    }
}
