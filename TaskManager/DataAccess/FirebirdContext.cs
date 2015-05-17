using System.Data.Entity;
using FirebirdSql.Data.FirebirdClient;

namespace ch.jaxx.TaskManager.DataAccess
{
	[DbConfigurationType(typeof(FirebirdContextConfiguration))]
	class FirebirdContext : DbContext
	{
		public FirebirdContext(string connString)
			//: base(new FbConnection(@"database=localhost:test.fdb;user=sysdba;password=masterkey"), true)
            : base(new FbConnection(connString), true)
            //server type=Embedded;user id=SYSDBA;password=MASTERKEY;dialect=3;character set=UTF8;client library=D:\WORK\Playground\FbTest\FbTest\bin\Debug\fbembed.dll;initial catalog=D:\task.fdb
		{ }

        public FirebirdContext()
           : base("name=DoEntities")

        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var monDatabaseConfiguration = modelBuilder.Entity<MONDatabase>();
            //monDatabaseConfiguration.HasKey(x => x.DatabaseName);
            //monDatabaseConfiguration.Property(x => x.DatabaseName).HasColumnName("MON$DATABASE_NAME");
            //monDatabaseConfiguration.Property(x => x.CreationDate).HasColumnName("MON$CREATION_DATE");
            //monDatabaseConfiguration.ToTable("MON$DATABASE");
        }

		//public DbSet<MONDatabase> MONDatabase { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TaskPhaseModel> TaskPhases { get; set; }
        public DbSet<ch.jaxx.TaskManager.DbMigration.DbVersionModel> DbVersion { get; set; }
	}
}
