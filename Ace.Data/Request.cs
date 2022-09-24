namespace Ace.Data
{
  public class Request
  {
    public int RequestId { get; set; }
    public string? AccessNumber { get; set; }

    public virtual List<PrescribedTest> PrescribedTests { get; set; }

    public Request()
    {
      PrescribedTests = new List<PrescribedTest>();
    }
  }
}