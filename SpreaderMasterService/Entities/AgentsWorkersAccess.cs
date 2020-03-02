using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class AgentsWorkersAccess
    {
        public int Workerid { get; set; }
        public int Accessid { get; set; }
        public DateTime? Created { get; set; }

        public virtual JobsAccess Access { get; set; }
        public virtual AgentsWorkers Worker { get; set; }
    }
}
