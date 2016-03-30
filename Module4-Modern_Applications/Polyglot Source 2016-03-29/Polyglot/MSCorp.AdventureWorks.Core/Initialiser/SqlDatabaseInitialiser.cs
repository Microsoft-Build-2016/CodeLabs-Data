using Common;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Core.Initialiser
{
    public class SqlDatabaseInitialiser
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageInitialiser"/> class.
        /// </summary>
        public SqlDatabaseInitialiser(string connectionString)
        {
            Argument.CheckIfNullOrEmpty(connectionString, "connectionString");
            _connectionString = connectionString;
        }

        public void CreateDatabase()
        {
            using (var context = new OrderDataContext(_connectionString))
            {
                // this will create the database with the schema from the Entity Model
                context.Database.CreateIfNotExists();
            }
        }
    }
}