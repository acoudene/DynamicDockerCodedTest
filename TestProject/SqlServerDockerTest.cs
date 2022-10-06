using Ace.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
    public class SqlServerDockerTest : IAsyncLifetime
    {
        private const string ZlicenseName = "";
        private const string ZlicenseKey = "";

        private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration()
            {
                Password = "yourStrong(!)Password"
            })
            .Build();

        [Fact]
        public void ReadPatients_Ok()
        {
            // Arrange
            var patients = default(List<Patient>);
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(_testcontainers.ConnectionString);

            // Act
            using (var context = new AceContext(builder.Options))
            {
                context.Database.EnsureCreated();
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

            builder.UseSqlServer(_testcontainers.ConnectionString);

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
            //Z.EntityFramework.Extensions.LicenseManager.AddLicense(ZlicenseName, ZlicenseKey);
            SeedData();
        }

        protected void SeedData()
        {
            var patients = SeedDataHelper.GeneratePatients();

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(_testcontainers.ConnectionString);

            using (var context = new AceContext(builder.Options))
            {
                context.Database.EnsureCreated();

                context.Patients!.AddRange(patients.ToArray());
                context.BulkSaveChanges();
            }
        }

        public Task DisposeAsync()
        {
            return _testcontainers.DisposeAsync().AsTask();
        }
    }
}