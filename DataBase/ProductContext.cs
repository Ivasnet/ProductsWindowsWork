using DataBase.Models;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace DataBase
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("ProductContext") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<ProductContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer); 

            var model = modelBuilder.Build(Database.Connection);
            var sqlGenerator = new SqliteSqlGenerator();
            _ = sqlGenerator.Generate(model.StoreModel);
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCounter> ProductsCounters { get; set; }
    }
}
