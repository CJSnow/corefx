using System.Collections.Generic;
using Xunit;

namespace System.Data.Odbc.Tests
{
    public class DataTypeTests
    {
        private const string connectionString =
            "Driver={SQL Server Native Client 11.0};" +
            "Server=(localdb)\\MSSQLLocalDB;" +
            "Database=master;" +
            "Trusted_Connection=Yes;";

        [Fact]
        public void DataTypesTest()
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
                        @"CREATE TABLE TestTable (
                            SomeByte TINYINT,
                            SomeBoolean BIT,
                            SomeDate DATE,
                            SomeDateTime DATETIME,
                            SomeDecimal DECIMAL(10,5),
                            SomeDouble FLOAT,
                            SomeSingle REAL,
                            SomeGuid UNIQUEIDENTIFIER,
                            SomeInt INT,
                            SomeLong BIGINT,
                            SomeString NVARCHAR(100))";

                    dbcmd.ExecuteNonQuery();
                    dbcmd.CommandText =
                        @"INSERT INTO TestTable (
                            SomeByte,
                            SomeBoolean,
                            SomeDate,
                            SomeDateTime,
                            SomeDecimal,
                            SomeDouble,
                            SomeSingle,
                            SomeGuid,
                            SomeInt,
                            SomeLong,
                            SomeString)
                        VALUES (
                            7,
                            1,
                            '2010-12-13',
                            '2016-02-29 22:33:44',
                            12345.12002,
                            1.0 + 0.00000001,
                            1.0 + 0.00000001,
                            '9b7c0b33-d38b-4d89-a3b2-0202c55ce6e5',
                            32767532,
                            2147483647,
                            'SomeString')";
                    dbcmd.ExecuteNonQuery();

                    SelectTestHelper.AssertEquals(
                        dbcmd,
                        "TestTable",
                        new Dictionary<string, object> {
                            { "SomeByte", (byte)7 },
                            { "SomeBoolean", true },
                            { "SomeDate", new DateTime(2010, 12, 13) },
                            { "SomeDateTime", new DateTime(2016, 2, 29, 22, 33, 44) },
                            { "SomeDecimal", 12345.12002m },
                            { "SomeDouble", 1.00000001d },
                            { "SomeSingle", 1f },
                            { "SomeGuid", new Guid("9b7c0b33-d38b-4d89-a3b2-0202c55ce6e5") },
                            { "SomeInt", 32767532 },
                            { "SomeLong", 2147483647L },
                            { "SomeString", "SomeString" },
                        });

                    // not calling .Commit() will automatically rollback the transaction.
                }
            }
        }
    }
}
