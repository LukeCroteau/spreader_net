using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class AgentsLog
    {
        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public int? Agentid { get; set; }
        public int? Jobid { get; set; }
        public int? Taskid { get; set; }
        public int? Workerid { get; set; }
        public int? LogType { get; set; }
        public string Message { get; set; }

        public virtual Agents Agent { get; set; }
        public virtual Jobs Job { get; set; }
        public virtual AgentsLogTypes LogTypeNavigation { get; set; }
        public virtual AgentsWorkers Worker { get; set; }
    }
}
