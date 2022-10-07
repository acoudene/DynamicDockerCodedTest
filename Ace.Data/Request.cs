using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ace.Data
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public string? AccessNumber { get; set; }

        public virtual List<PrescribedTest> PrescribedTests { get; set; }

        public Request()
        {
            PrescribedTests = new List<PrescribedTest>();
        }

        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
    }
}