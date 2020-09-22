using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_00.Data
{
    public class JobModel
    {
        public int Id { get; set; }
        public JobStatuses Status { get; set; }
        public string Detail { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
        public enum JobStatuses
        {
            Todo,
            Started,
            Completed
        }
}
