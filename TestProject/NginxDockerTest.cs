using System.Net;

namespace TestProject
{
    /// <summary>
    /// SqlServer docker Tester
    /// </summary>
    public class NginxDockerTest : IAsyncLifetime
    {
        private const int Port = 8889;
        /// <summary>
        /// Container
        /// </summary>
        private readonly TestcontainersContainer _testcontainers = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("nginx")
            .WithName("nginx")
            .WithPortBinding(Port, 80)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .Build();

        private readonly ITestOutputHelper _output;
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public NginxDockerTest(ITestOutputHelper output)
        {
            _output = output;
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        [Fact]        
        public void NgingRequest_Ok()
        {
            // Just to illustrate, set a breakpoint here
            // Arrange

            // Act        
            var client = new HttpClient();
            var response = client.GetAsync($"http://localhost:{Port}");

            // Assert
            Assert.NotNull(response);
        }
        

        /// <summary>
        /// Initialize test class
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            await _testcontainers.StartAsync();
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