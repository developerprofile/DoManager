using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ch.jaxx.TaskManager.DataAccess
{
    [Table("TASKS")]
    public class TaskModel : ITask
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Column("NAME")]
        [MaxLength(256)]
        public string Name { get; set; }

        [Column("CREATIONDATE")]        
        public DateTime? CreationDate { get; set; }

        [Column("STARTDATE")]
        public DateTime? StartDate { get; set; }

        [Column("DONEDATE")]
        public DateTime? DoneDate { get; set; }

        [Column("STATE")]        
        public TaskState? State { get; set; }
    }
}
