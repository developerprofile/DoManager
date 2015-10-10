using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ch.jaxx.TaskManager.DataAccess
{
    [Table("TASKPHASE")]
    public class TaskPhaseModel : ITaskPhase
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Column("TASKID")]
        public int TaskId { get; set; }
        
        [Column("STARTDATE")]
        public DateTime? StartDate { get; set; }
        
        [Column("ENDDATE")]
        public DateTime? EndDate { get; set; }
    }
}
