using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ch.jaxx.TaskManager.DataAccess
{
    [Table("TASKS")]
    public class TaskModel
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        
        [Column("NAME")]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
