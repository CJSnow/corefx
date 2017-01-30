// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Xunit;

namespace System.Data.Odbc.Tests
{
    public class OdbcConnectionTests
    {
        [Fact]
        public void SuccessfulConnectionTest()
        {
            const string connectionString =
                "Driver={SQL Server Native Client 11.0};" +
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=master;" +
                "Trusted_Connection=Yes;";
            OdbcConnection connection = new OdbcConnection(connectionString);
            connection.Open();
            Assert.Equal(ConnectionState.Open, connection.State);
            Assert.Equal("sqlncli11.dll", connection.Driver);
            Assert.Equal("master", connection.Database);
        }

        [Fact]
        public void WrongDriverTest()
        {
            const string connectionString =
                "Driver={Wrong Driver};" +
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=master;" +
                "Trusted_Connection=Yes;";
            OdbcConnection connection = new OdbcConnection(connectionString);
            var exception = Record.Exception(() => connection.Open());
            Assert.NotNull(exception);
            Assert.IsType<OdbcException>(exception);
            Assert.Equal(
                "ERROR [IM002] [Microsoft][ODBC Driver Manager] Data source name not found and no default driver specified",
                exception.Message);
        }

        //[Fact]
        //[OuterLoop("Connection has to time out and takes a while")]
        public void WrongServerTest()
        {
            const string connectionString =
                "Driver={SQL Server Native Client 11.0};" +
                "Server=WrongServer;" +
                "Database=master;" +
                "Trusted_Connection=Yes;";
            OdbcConnection connection = new OdbcConnection(connectionString);
            var exception = Record.Exception(() => connection.Open());
            Assert.NotNull(exception);
            Assert.IsType<OdbcException>(exception);
            Assert.Equal(
                "ERROR [08001] [Microsoft][SQL Server Native Client 11.0]Named Pipes Provider: Could not open a connection to SQL Server [53]. \r\nERROR [HYT00] [Microsoft][SQL Server Native Client 11.0]Login timeout expired\r\nERROR [08001] [Microsoft][SQL Server Native Client 11.0]A network-related or instance-specific error has occurred while establishing a connection to SQL Server. Server is not found or not accessible. Check if instance name is correct and if SQL Server is configured to allow remote connections. For more information see SQL Server Books Online.",
                exception.Message);
        }

        [Fact]
        public void WrongCredentialsTest()
        {
            const string connectionString =
                "Driver={SQL Server Native Client 11.0};" +
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=master;" +
                "Uid=WrongUsername;" +
                "Pwd=WrongPassword;";
            OdbcConnection connection = new OdbcConnection(connectionString);
            var exception = Record.Exception(() => connection.Open());
            Assert.NotNull(exception);
            Assert.IsType<OdbcException>(exception);
            Assert.Equal(
                "ERROR [28000] [Microsoft][SQL Server Native Client 11.0][SQL Server]Login failed for user 'WrongUsername'.\r\nERROR [28000] [Microsoft][SQL Server Native Client 11.0][SQL Server]Login failed for user 'WrongUsername'.",
                exception.Message);
        }
    }
}
