namespace TestProject
{
    /// <summary>
    /// Oracle docker Tester
    /// </summary>
    public class OracleDockerTest : IAsyncLifetime
    {
        /// <summary>
        /// Container
        /// </summary>
        private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<OracleTestcontainer>()
            .WithDatabase(new OracleTestcontainerConfiguration())
            .Build();

        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public OracleDockerTest(ITestOutputHelper output)
        {
            _output = output;
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        [Fact]
        public void ReadPatients_Ok()
        {
            // Arrange
            var patients = default(List<Patient>);
            var builder = GetDbContextBuilder();

            // Act
            using (var context = new AceContext(builder.Options))
            {
                patients = context.Patients?.ToList();

            }

            // Assert
            Assert.NotNull(patients);
            Assert.True(patients?.All(patient => patient.PatientId > 0));

            patients!.ForEach(patient => _output.WriteLine($"Patient : {patient.Name}"));
        }

        [Fact]
        public void ReadPatientsWithRequests_Ok()
        {
            // Arrange
            var patients = default(List<Patient>);
            var builder = GetDbContextBuilder();

            // Act
            using (var context = new AceContext(builder.Options))
            {
                patients = context.Patients?
                    .Include(_ => _.Requests)
                    .ToList();
            }

            // Assert
            Assert.NotNull(patients);
            Assert.True(patients?.All(patient => patient.PatientId > 0));
            Assert.True(patients?.All(patient => patient.Requests != null && patient.Requests.Any()));

            patients!.ForEach(patient =>
                _output.WriteLine($"Patient : {patient.Name} - Requests : [{string.Join(", ", patient.Requests.Select(request => request.AccessNumber))}"));
        }

        [Fact]
        public void ReadPrescribedTests_Ok()
        {
            // Arrange
            var prescribedTests = default(List<PrescribedTest>);
            var builder = GetDbContextBuilder();

            // Act
            using (var context = new AceContext(builder.Options))
            {
                prescribedTests = context.PrescribedTests?.ToList();
            }

            // Assert
            Assert.NotNull(prescribedTests);
            Assert.True(prescribedTests?.All(prescribedTest => prescribedTest.PrescribedTestId > 0));

            prescribedTests!.ForEach(prescribedTest => _output.WriteLine($"Prescribed test : {prescribedTest.Code}"));
        }

        public async Task InitializeAsync()
        {
            await _testcontainers.StartAsync();

            //string name = _config["ZEntityframework:ZlicenseName"];
            //string key = _config["ZEntityframework:ZlicenseKey"];
            //Z.EntityFramework.Extensions.LicenseManager.AddLicense(name, key);
            SeedData();
        }

        /// <summary>
        /// Create database if needed and Fill data
        /// </summary>
        protected void SeedData()
        {
            var patients = SeedDataHelper.GenerateData();
            var builder = GetDbContextBuilder();

            using (var context = new AceContext(builder.Options))
            {
                context.Database.EnsureCreated();

                context.Patients!.AddRange(patients.ToArray());

                // context.SaveChanges();
                // Replace above line and use this line to optimize
                /// IncludeGraph allows you to INSERT/UPDATE/MERGE entities by including the child entities graph.
                /// <see cref="https://entityframework-extensions.net/efcore-devart-oracle-provider"/>
                context.BulkSaveChanges(options =>
                {
                    options.Log = s => _output.WriteLine(s);
                    options.IncludeGraph = true;
                });                
            }
        }

        /// <summary>
        /// Get settings for EF Context
        /// </summary>
        /// <returns></returns>
        protected DbContextOptionsBuilder GetDbContextBuilder()
        {
            var builder = new DbContextOptionsBuilder();
            builder
                .UseOracle(GetConnectionString())
                .LogTo(_output.WriteLine, LogLevel.Information);
            return builder;
        }

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <returns></returns>
        protected string GetConnectionString()
        {
            string key = _config["Devart:LicenseKey"];
            return $"{_testcontainers.ConnectionString};Direct=true;SID=xe;License Key={key}";
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