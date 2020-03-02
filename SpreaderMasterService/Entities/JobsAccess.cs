using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class JobsAccess
    {
        public JobsAccess()
        {
            AgentsWorkersAccess = new HashSet<AgentsWorkersAccess>();
            JobsCron = new HashSet<JobsCron>();
            Tasks = new HashSet<Tasks>();
        }

        public int Id { get; set; }
        public int Jobid { get; set; }
        public DateTime? Created { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual Jobs Job { get; set; }
        public virtual ICollection<AgentsWorkersAccess> AgentsWorkersAccess { get; set; }
        public virtual ICollection<JobsCron> JobsCron { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
