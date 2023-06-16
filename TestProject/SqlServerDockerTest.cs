namespace TestProject;

/// <summary>
/// SqlServer docker Tester
/// </summary>
public class SqlServerDockerTest : IAsyncLifetime
{
  /// <summary>
  /// Container
  /// </summary>
  private readonly TestcontainerDatabase _testcontainers = new TestcontainersBuilder<MsSqlTestcontainer>()
      .WithDatabase(new MsSqlTestcontainerConfiguration()
      {
        Password = "yourStrong(!)Password"
      })
      .Build();

  private readonly ITestOutputHelper _output;
  private readonly IConfiguration _config;

  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="output"></param>
  public SqlServerDockerTest(ITestOutputHelper output)
  {
    _output = output;
    _config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", false, true)
        .Build();
  }

  /// <summary>
  /// Read all patients
  /// </summary>
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

  /// <summary>
  /// Read all patients with requests 
  /// </summary>
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

  /// <summary>
  /// Read all prescribed tests
  /// </summary>
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


  /// <summary>
  /// Read all patients with requests 
  /// </summary>
  [Fact]
  public void ComparePatientsWithRequests_WithDb_Ok()
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
  /// <summary>
  /// Initialize test class
  /// </summary>
  /// <returns></returns>
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

      context.SaveChanges();
      // Replace above line and use this line to optimize
      // context.BulkSaveChanges(options => options.Log = s => _output.WriteLine(s));                
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
        .UseSqlServer(GetConnectionString())
        .LogTo(_output.WriteLine, LogLevel.Information);
    return builder;
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