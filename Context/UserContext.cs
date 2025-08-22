using Microsoft.EntityFrameworkCore;

namespace UserCRUD.Context
{
    public class UserContext : DbContext
    {
        public DbSet<Models.Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=person.sqlite");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
