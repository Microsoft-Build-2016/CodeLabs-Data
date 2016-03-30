using System.Data.Entity;
using Common;

namespace MSCorp.AdventureWorks.Core.Repository
{
    public static class EntityFrameworkExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            Argument.CheckIfNull(dbSet, "dbSet");
            dbSet.RemoveRange(dbSet);
        }
    }
}