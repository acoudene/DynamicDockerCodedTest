using DotNet.Testcontainers.Containers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System.Net;

namespace TestProject
{
    /// <summary>
    /// SqlServer docker Tester
    /// </summary>
    public class MongoDbDockerTest : IAsyncLifetime
    {
        private const int Port = 8889;
        /// <summary>
        /// Container
        /// </summary>
        private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<MongoDbTestcontainer>()
            .WithDatabase(new MongoDbTestcontainerConfiguration()
            {
                Database = "MyDatabase",
                Username = "Admin",
                Password = "Passw0rd"
                
            })
            .Build();

        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public MongoDbDockerTest(ITestOutputHelper output)
        {
            _output = output;
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        private class PatientMongoDb : Patient
        {
            [BsonId(IdGenerator = typeof(CombGuidGenerator))]
            public Guid Id { get; set; }
        }

        /// <summary>
        /// Read all prescribed tests
        /// </summary>
        [Fact]
        public void ReadPatients_Ok()
        {
            // Arrange
            var patients = default(List<PatientMongoDb>);
            var connectionString = GetConnectionString();

            var collection = new MongoClient(connectionString)
                .GetDatabase(_testcontainers.Database)?
                .GetCollection<PatientMongoDb>("Patient");

            // Act            
            patients = collection
                .AsQueryable<PatientMongoDb>()
                .ToList();

            // Assert
            Assert.NotNull(patients);
            Assert.True(patients?.All(patient => patient.PatientId >= 0));

            patients!.ForEach(patient => _output.WriteLine($"Patient : {patient.Name}"));
        }

        /// <summary>
        /// Initialize test class
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await _testcontainers.StartAsync();

            SeedData();
        }

        /// <summary>
        /// Create database if needed and Fill data
        /// </summary>
        protected void SeedData()
        {
            var patients = SeedDataHelper.GenerateData()
                .Select(p => new PatientMongoDb()
                {
                    Name = p.Name,
                    FirstName = p.FirstName,
                    PatientId = p.PatientId                    
                });

            var connectionString = GetConnectionString();

            var collection = new MongoClient(connectionString)
                .GetDatabase(_testcontainers.Database)?
                .GetCollection<PatientMongoDb>("Patient");

            collection?.InsertMany(patients);
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <returns></returns>
        protected string GetConnectionString()
        {
            return _testcontainers.ConnectionString;
        }

        /// <summary>
        /// Clean instance
        /// </summary>
        /// <returns></returns>
        public Task DisposeAsync()
        {
            return _testcontainers.DisposeAsync().AsTask();
        }
    }
}