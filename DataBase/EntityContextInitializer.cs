using SQLite.CodeFirst;
using System.Data.Entity;

namespace DataBase
{
    public class EntityContextInitializer : SqliteDropCreateDatabaseAlways<ProductContext>
    {
        public EntityContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder) { }

        protected override void Seed(ProductContext context)
        {
            base.Seed(context);
        }
    }
}
