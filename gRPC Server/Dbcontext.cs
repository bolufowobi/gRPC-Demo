using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace gRPC_Server
{
    class DbprofilDbcontext:DbContext
    {
        public DbSet<DbProfile> profiles { get; set; }
    }
}
