using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class AgentsWorkers
    {
        public AgentsWorkers()
        {
            AgentsLog = new HashSet<AgentsLog>();
            AgentsWorkersAccess = new HashSet<AgentsWorkersAccess>();
            Tasks = new HashSet<Tasks>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public bool? Active { get; set; }
        public DateTime? Lastping { get; set; }
        public int? Agentid { get; set; }
        public int? Jobid { get; set; }
        public string Version { get; set; }

        public virtual Agents Agent { get; set; }
        public virtual Jobs Job { get; set; }
        public virtual ICollection<AgentsLog> AgentsLog { get; set; }
        public virtual ICollection<AgentsWorkersAccess> AgentsWorkersAccess { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
