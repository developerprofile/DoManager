using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ch.jaxx.TaskManager.DbMigration
{
    [Table("DBVERSION")]
    public class DbVersionModel
    {
        [Key]
        [Column("DBVERSION")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Version { get; set; }

        [Column("DESCRIPTION")]
        public string Descripiton { get; set; }
    }
}
