using System;
using System.Collections.Generic;

namespace SpreaderMasterService.Entities
{
    public partial class JobsCron
    {
        public int Id { get; set; }
        public int? Jobid { get; set; }
        public DateTime? Created { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }
        public string Daysofweek { get; set; }
        public int Dayofmonth { get; set; }
        public TimeSpan Starttime { get; set; }
        public int? Accessid { get; set; }
        public string Taskkey { get; set; }
        public string Params { get; set; }
        public DateTime? LastRun { get; set; }
        public int? LastTaskid { get; set; }

        public virtual JobsAccess Access { get; set; }
        public virtual Jobs Job { get; set; }
        public virtual Tasks LastTask { get; set; }
    }
}
