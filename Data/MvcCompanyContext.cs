using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcCompany.Models;

namespace MvcCompany.Data
{
    public class MvcCompanyContext : DbContext
    {
        public MvcCompanyContext (DbContextOptions<MvcCompanyContext> options)
            : base(options)
        {
        }

        public DbSet<MvcCompany.Models.Employees>? Employees { get; set; }
    }
}
