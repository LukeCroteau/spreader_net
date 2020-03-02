using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class Agents
    {
        public Agents()
        {
            AgentsLog = new HashSet<AgentsLog>();
            AgentsWorkers = new HashSet<AgentsWorkers>();
            Tasks = new HashSet<Tasks>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public bool? Active { get; set; }
        public DateTime? Lastping { get; set; }
        public string Name { get; set; }
        public string Netname { get; set; }
        public string Version { get; set; }
        public int? Cpucount { get; set; }
        public int? Totalmemory { get; set; }

        public virtual ICollection<AgentsLog> AgentsLog { get; set; }
        public virtual ICollection<AgentsWorkers> AgentsWorkers { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
