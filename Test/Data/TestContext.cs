using Microsoft.EntityFrameworkCore;

namespace Test.Data
{
    public class TestContext : DbContext
    {
        public TestContext (DbContextOptions<TestContext> options)
            : base(options)
        {
        }

        public DbSet<Test.Models.Product> Product { get; set; }
        public DbSet<Test.Models.Category> Category { get; set; }
        public DbSet<Test.Models.CategoryField> CategoryField { get; set; }
        public DbSet<Test.Models.FieldValuePair> FieldValuePair { get; set; }
    }
}
