using System.Collections.Generic;
using Xunit;

namespace System.Data.Odbc.Tests
{
    public class SmokeTest
    {
        private const string connectionString =
            "Driver=SQLite3;" +
            "Database=smoketests.sqlite;";

        [Fact]
        public void SmokeTest()
        {
            using (var dbcon = new OdbcConnection(connectionString))
            {
                dbcon.Open();
                using (var transaction = dbcon.BeginTransaction())
                using (var dbcmd = dbcon.CreateCommand())
                {
                    dbcmd.Transaction = transaction;

                    dbcmd.CommandText =
                        @"CREATE TABLE SomeTable (
                            SomeByte TINYINT,
                            SomeBoolean BIT,
                            SomeDate DATE,
                            SomeDateTime DATETIME,
                            SomeDecimal DECIMAL(10,5),
                            SomeDouble FLOAT,
                            SomeFloat REAL,
                            SomeGuid UNIQUEIDENTIFIER,
                            SomeInt32 INT,
                            SomeInt64 BIGINT,
                            SomeString NVARCHAR(100))";

                    dbcmd.ExecuteNonQuery();
                    dbcmd.CommandText =
                        @"INSERT INTO SomeTable (
                            SomeByte,
                            SomeBoolean,
                            SomeDate,
                            SomeDateTime,
                            SomeDecimal,
                            SomeDouble,
                            SomeFloat,
                            SomeGuid,
                            SomeInt32,
                            SomeInt64,
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

                    dbcmd.CommandText =
                        @"SELECT 
                            SomeByte,
                            SomeBoolean,
                            SomeDate,
                            SomeDateTime,
                            SomeDecimal,
                            SomeDouble,
                            SomeFloat,
                            SomeGuid,
                            SomeInt32,
                            SomeInt64,
                            SomeString
                        FROM SomeTable";
                    using (var reader = dbcmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.Equal((byte)7, reader.GetByte(0));
                        Assert.Equal(true, reader.GetBoolean(1));
                        Assert.Equal(new DateTime(2010, 12, 13), reader.GetDate(2));
                        Assert.Equal(new DateTime(2016, 2, 29, 22, 33, 44), reader.GetDateTime(3));
                        Assert.Equal(12345.12002m, reader.GetDecimal(4));
                        Assert.Equal(1.00000001d, reader.GetDouble(5));
                        Assert.Equal(1f, reader.GetFloat(6));
                        // TODO[tinchou]: test Guid reader
                        //Assert.Equal(new Guid("9b7c0b33-d38b-4d89-a3b2-0202c55ce6e5"), reader.GetGuid(7));
                        Assert.Equal(32767532, reader.GetInt32(8));
                        Assert.Equal(2147483647L, reader.GetInt64(9));
                        Assert.Equal("SomeString", reader.GetString(10));
                    }
                    // not calling .Commit() will automatically rollback the transaction.
                }
            }
        }
    }
}
