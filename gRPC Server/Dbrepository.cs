using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace gRPC_Server
{
  class Dbrepository
    {
        public List<DbProfile> GetProfiles()
        {
            DbprofilDbcontext dbPrdbc = new DbprofilDbcontext();
            return dbPrdbc.profiles.ToList();

        }

    }
}
