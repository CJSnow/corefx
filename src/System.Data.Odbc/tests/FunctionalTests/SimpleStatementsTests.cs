using System.Collections.Generic;
using Xunit;

namespace System.Data.Odbc.Tests
{
    public class SimpleStatementsTests
    {
        private const string connectionString =
            "Driver={SQL Server Native Client 11.0};" +
            "Server=(localdb)\\MSSQLLocalDB;" +
            "Database=master;" +
            "Trusted_Connection=Yes;";

        [Fact]
        public void SelectDatabasesTest()
        {
            using (var dbcon = new OdbcConnection(connectionString))
            {
                dbcon.Open();
                using (var dbcmd = dbcon.CreateCommand())
                {
                    SelectTestHelper.AssertEquals(
                        dbcmd,
                        "sys.databases",
                        new Dictionary<string, object> { { "database_id", 1 }, { "name", "master" } },
                        new Dictionary<string, object> { { "database_id", 2 }, { "name", "tempdb" } },
                        new Dictionary<string, object> { { "database_id", 3 }, { "name", "model" } },
                        new Dictionary<string, object> { { "database_id", 4 }, { "name", "msdb" } });
                }
            }
        }

        [Fact]
        public void CreateDatabaseTest()
        {
            using (var dbcon = new OdbcConnection(connectionString))
            {
                dbcon.Open();
                using (var transaction = dbcon.BeginTransaction())
                using (var dbcmd = dbcon.CreateCommand())
                {
                    // Assign transaction object for a pending local transaction.
                    dbcmd.Transaction = transaction;

                    dbcmd.CommandText =
                        "CREATE TABLE Region (RegionID INT, RegionDescription nvarchar(100))";
                    dbcmd.ExecuteNonQuery();
                    dbcmd.CommandText =
                        "INSERT INTO Region (RegionID, RegionDescription) VALUES (101, 'Description')";
                    dbcmd.ExecuteNonQuery();

                    SelectTestHelper.AssertEquals(
                        dbcmd,
                        "Region",
                        new Dictionary<string, object> { { "RegionId", 101 }, { "RegionDescription", "Description" } });

                    // not calling .Commit() will automatically rollback the transaction.
                }
            }
        }
    }
}
