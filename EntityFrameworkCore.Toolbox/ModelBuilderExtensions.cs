using EntityFrameworkCore.Toolbox.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace EntityFrameworkCore.Toolbox
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Fix an issue with EF setitng the varchar defaulting to size 1 rather than max.
        /// Use in OnModelCreating.
        /// </summary>
        /// <param name="modelBuilder">The context modelbuilder.</param>
        public static void FixVarCharMaxLength(this ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
            {
                if (!string.Equals(nameof(SqlDbType.VarChar), property.GetColumnType(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var lengths = ((MaxLengthAttribute[])property.PropertyInfo.GetCustomAttributes(typeof(MaxLengthAttribute), false)).Select(m => m.Length)
                    .Concat(((StringLengthAttribute[])property.PropertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), false)).Select(e => e.MaximumLength));

                if (lengths.Any(ml => ml > 0 && ml < int.MaxValue))
                {
                    continue;
                }

                property.SetColumnType($"{SqlDbType.VarChar}(MAX)");
            }
        }

        /// <summary>
        /// Creates a reference table in the database for the specified enum type.
        /// Use in OnModelCreating.
        /// </summary>
        /// <param name="modelBuilder">The context modelbuilder.</param>
        public static void AddEnumReferenceTable<TEnum>(this ModelBuilder modelBuilder, string tablePrefix = "EnumRef", bool useFullName = false)
            where TEnum : struct, Enum
        {
            var name = useFullName ? typeof(TEnum).FullName?.Replace(".", string.Empty) ?? typeof(TEnum).Name : typeof(TEnum).Name;
            var entityBuilder = modelBuilder.SharedTypeEntity<EnumReference>($"{tablePrefix}{name}");
            entityBuilder.HasData(Enum.GetValues<TEnum>().Select((e, i) => new EnumReference { Id = i + 1, Name = e.ToString(), Value = Convert.ToInt64(e) }));
        }

        /// <summary>
        /// Creates a reference table in the database for the specified enum type.
        /// Use in OnModelCreating.
        /// </summary>
        /// <param name="modelBuilder">The context modelbuilder.</param>
        public static void AddEnumReferenceTable(this ModelBuilder modelBuilder, Type type, string tablePrefix = "EnumRef", bool useFullName = false)
        {
            if (!type.IsEnum) throw new Exception($"Cannot add non-enum type {type.Name} as an enum reference to the database.");
            var name = useFullName ? type.FullName?.Replace(".", string.Empty) ?? type.Name : type.Name;
            var entityBuilder = modelBuilder.SharedTypeEntity<EnumReference>($"{tablePrefix}{name}");
            entityBuilder.HasData(Enum.GetValues(type).Cast<Enum>().Select((e, i) => new EnumReference { Id = i + 1, Name = e.ToString(), Value = Convert.ToInt64(e) }));
        }

        /// <summary>
        /// Creates reference tables in the database for all enums in the executing assembly.
        /// Use in OnModelCreating.
        /// </summary>
        /// <param name="modelBuilder">The context modelbuilder.</param>
        public static void AddAllEnumReferenceTables(this ModelBuilder modelBuilder, string tablePrefix = "EnumRef", bool useFullName = false)
        {
            var enums = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum);
            
            foreach(var enumType in enums)
            {
                AddEnumReferenceTable(modelBuilder, enumType, tablePrefix, useFullName);
            }
        }
    }
}
