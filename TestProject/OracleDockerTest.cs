using Ace.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
    public class OracleDockerTest : IAsyncLifetime
    {
        private const string LicenseKey = "";
        private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<OracleTestcontainer>()
            .WithDatabase(new OracleTestcontainerConfiguration())
            .Build();

        [Fact]
        public void ReadPatients_Ok()
        {
            // Arrange
            var patients = default(List<Patient>);
            var builder = new DbContextOptionsBuilder();

            builder.UseOracle(GetConnectionString());

            // Act
            using (var context = new AceContext(builder.Options))
            {
                patients = context.Patients?.ToList();
            }

            // Assert
            Assert.NotNull(patients);
            Assert.True(patients?.All(patient => patient.PatientId > 0));
        }

        [Fact]
        public void ReadPrescribedTests_Ok()
        {
            // Arrange
            var prescribedTests = default(List<PrescribedTest>);
            var builder = new DbContextOptionsBuilder();

            builder.UseOracle(GetConnectionString());

            // Act
            using (var context = new AceContext(builder.Options))
            {
                prescribedTests = context.PrescribedTests?.ToList();
            }

            // Assert
            Assert.NotNull(prescribedTests);
            Assert.True(prescribedTests?.All(prescribedTest => prescribedTest.PrescribedTestId > 0));
        }

        public async Task InitializeAsync()
        {
            await _testcontainers.StartAsync();
            SeedData();
        }

        protected void SeedData()
        {
            var patients = SeedDataHelper.GeneratePatients();

            var builder = new DbContextOptionsBuilder();
            builder.UseOracle(GetConnectionString());

            using (var context = new AceContext(builder.Options))
            {
                context.Database.EnsureCreated();

                context.Patients!.AddRange(patients.ToArray());
                context.SaveChanges();
            }
        }

        protected string GetConnectionString()
        {
            return $"{_testcontainers.ConnectionString};Direct=true;SID=xe;License Key={LicenseKey}";
        }

        public Task DisposeAsync()
        {
            return _testcontainers.DisposeAsync().AsTask();
        }
    }
}