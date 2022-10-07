using Ace.Data;

namespace TestProject
{
    /// <summary>
    /// Generate Data
    /// </summary>
    internal static class SeedDataHelper
    {
        /// <summary>
        /// Generate patients with requests and prescribed tests
        /// </summary>
        /// <returns></returns>
        public static List<Patient> GeneratePatients()
        {
            var patients = new List<Patient>();
            for (int i = 0; i < 10; i++)
            {
                var patient = new Patient()
                {
                    FirstName = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString()
                };
                for (int j = 0; j < 10; j++)
                {
                    var request = new Request()
                    {
                        AccessNumber = Guid.NewGuid().ToString()
                    };
                    for (int k = 0; k < 10; k++)
                    {
                        var test = new PrescribedTest()
                        {
                            Code = Guid.NewGuid().ToString()
                        };
                        request.PrescribedTests.Add(test);
                    }
                    patient.Requests.Add(request);
                }

                patients.Add(patient);
            }
            return patients;
        }
    }
}
