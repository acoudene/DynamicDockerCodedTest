using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Data.Common;
using System.Data.SqlClient;

namespace TestProject
{
  public class UnitTest1 : IAsyncLifetime
  {
    private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<MsSqlTestcontainer>()
        .WithDatabase(new MsSqlTestcontainerConfiguration()
        {
          Database = "MyTechnidataDb",
          Username = "acoudene",
          Password = "acoudene"
        })
        .Build();

    [Fact]
    public void Test1()
    {
      
      using (var connection = new SqlConnection(_testcontainers.ConnectionString))
      {
        using (var command = new SqlCommand())
        {
          connection.Open();
          command.Connection = connection;
          command.CommandText = "SELECT 1";
          command.ExecuteReader();
        }
      }
    }

    public Task InitializeAsync()
    {
      return _testcontainers.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _testcontainers.DisposeAsync().AsTask();
    }
  }
}