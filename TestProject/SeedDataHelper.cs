using Ace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
  internal static class SeedDataHelper
  {
    public static List<Patient> GeneratePatients()
    {
      var patients = new List<Patient>();
      for (int i = 0; i < 1000; i++)
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
