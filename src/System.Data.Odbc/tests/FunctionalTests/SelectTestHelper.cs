using System.Collections.Generic;
using Xunit;

namespace System.Data.Odbc.Tests
{
    public static class SelectTestHelper
    {
        public static void AssertEquals(OdbcCommand dbcmd, string v, params Dictionary<string, object>[] p)
        {
            dbcmd.CommandText =
                $"SELECT * FROM {v}";
            using (var reader = dbcmd.ExecuteReader())
            {
                foreach (var item in p)
                {
                    reader.Read();
                    foreach (var kvp in item)
                    {
                        Assert.Equal(kvp.Value, reader[kvp.Key]);
                    }
                }
            }
        }
    }
}
