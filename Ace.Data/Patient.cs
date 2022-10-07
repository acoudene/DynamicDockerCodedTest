using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ace.Data
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }

        public virtual List<Request> Requests { get; set; }

        public Patient()
        {
            Requests = new List<Request>();
        }
    }
}