using Microsoft.EntityFrameworkCore;
using TestConsole.DataModels;

namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public class AppDbContext : DbContext
    {
        public static string DefaultConnection = "Data Source=.;Initial Catalog=EfTools;Integrated Security=True;";

        public AppDbContext()
            : base(new DbContextOptionsBuilder<AppDbContext>()
                 .UseSqlServer(connectionString: DefaultConnection)
                 .Options)
        { }

        public DbSet<AppUser> Users { get; set; }
    }
}
