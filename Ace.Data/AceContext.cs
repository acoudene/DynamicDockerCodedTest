using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace.Data
{
  public class AceContext : DbContext
  {
    public DbSet<Patient>? Patients { get; set; }
    public DbSet<Request>? Requests { get; set; }
    public DbSet<PrescribedTest>? PrescribedTests { get; set; }

    public AceContext()
    {

    }
    public AceContext(DbContextOptions options)
      : base(options)
    {

    }
  }
}
