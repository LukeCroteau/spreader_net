using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class JobsLogTypes
    {
        public JobsLogTypes()
        {
            JobsLog = new HashSet<JobsLog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<JobsLog> JobsLog { get; set; }
    }
}
