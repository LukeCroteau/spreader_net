using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class Tasks
    {
        public Tasks()
        {
            JobsCron = new HashSet<JobsCron>();
        }

        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public int Jobid { get; set; }
        public string Taskkey { get; set; }
        public string Params { get; set; }
        public bool? Processed { get; set; }
        public bool? Processing { get; set; }
        public bool? ProcessedWithErrors { get; set; }
        public DateTime? Starttime { get; set; }
        public DateTime? Stoptime { get; set; }
        public int? Agentid { get; set; }
        public int? Workerid { get; set; }
        public int? Accessid { get; set; }

        public virtual JobsAccess Access { get; set; }
        public virtual Agents Agent { get; set; }
        public virtual Jobs Job { get; set; }
        public virtual AgentsWorkers Worker { get; set; }
        public virtual ICollection<JobsCron> JobsCron { get; set; }
    }
}
