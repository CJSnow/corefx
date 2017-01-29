using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.Odbc.Tests
{
    public class ConnectionTests
    {
        [Theory]
        [InlineData(0, "master (1)")]
        [InlineData(1, "tempdb (2)")]
        [InlineData(2, "model (3)")]
        [InlineData(3, "msdb (4)")]
        public void NorthwindTest(int index, string name)
        {
            var connectionString =
                "Driver={SQL Server Native Client 11.0};" +
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=master;" +
                "Trusted_Connection=Yes;";
            IDbConnection dbcon = new OdbcConnection(connectionString);
            dbcon.Open();
            using (IDbCommand dbcmd = dbcon.CreateCommand())
            {
                var sql =
                    "SELECT * " +
                    "FROM sys.databases";
                dbcmd.CommandText = sql;
                using (IDataReader reader = dbcmd.ExecuteReader())
                {
                    for (int i = 0; i < index + 1; i++)
                    {
                        reader.Read();
                    }

                    var actualName = (string)reader["name"];
                    var databaseId = (int)reader["database_id"];
                    Assert.Equal(name, $"{actualName} ({databaseId})");
                }
            }
            dbcon.Close();
            dbcon = null;
        }
    }
}
