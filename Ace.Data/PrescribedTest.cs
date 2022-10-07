using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ace.Data
{
    public class PrescribedTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrescribedTestId { get; set; }
        public string? Code { get; set; }

        public int RequestId { get; set; }

        [ForeignKey("RequestId")]
        public Request? Request { get; set; }
    }
}