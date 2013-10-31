using System.Data.Entity;

namespace WebApp.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("DefaultConnection")
        {

        }

        #region Tables

        public DbSet<User> Users { get; set; }

        #endregion

        #region Stored Procedures

        #endregion
    }
}