using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class Jobs
    {
        public Jobs()
        {
            AgentsLog = new HashSet<AgentsLog>();
            AgentsWorkers = new HashSet<AgentsWorkers>();
            JobsAccess = new HashSet<JobsAccess>();
            JobsCron = new HashSet<JobsCron>();
            JobsLog = new HashSet<JobsLog>();
            Tasks = new HashSet<Tasks>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public bool? Active { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Params { get; set; }

        public virtual ICollection<AgentsLog> AgentsLog { get; set; }
        public virtual ICollection<AgentsWorkers> AgentsWorkers { get; set; }
        public virtual ICollection<JobsAccess> JobsAccess { get; set; }
        public virtual ICollection<JobsCron> JobsCron { get; set; }
        public virtual ICollection<JobsLog> JobsLog { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
