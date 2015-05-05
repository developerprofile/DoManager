using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.jaxx.TaskManager.DataAccess;
using libjfunx.logging;

namespace ch.jaxx.TaskManager.DbMigration
{
    public class DbMigrator
    {
        public DbMigrator(string ConnectionString)
        {            
            this.context = new FirebirdContext(ConnectionString);
            this.connectionString = ConnectionString;
        }

        private string connectionString;
        private FirebirdContext context;

        private int GetDbVersion()
        {
            using (var context = new FirebirdContext(this.connectionString))
            {
                int dbVersion = 0;
                try
                {                    
                    dbVersion = context.DbVersion.Max(d => d.Version);
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException e)
                {
                    Logger.Log(LogEintragTyp.Fehler, e.InnerException.ToString());
                    if (e.InnerException.ToString().Contains("DBVERSION"))
                    {                        
                        context.Database.ExecuteSqlCommand("CREATE TABLE DBVERSION (DBVERSION int, DESCRIPTION varchar(256))");
                        context.SaveChanges();
                        context.DbVersion.Add(new DbVersionModel() { Version = 1, Descripiton = "Inital Db Version with version table" });
                        context.SaveChanges();
                        Logger.Log(LogEintragTyp.Erfolg, "Created Table DBVERSION");
                        UpgradeDb();
                    }
                }
                Logger.Log(LogEintragTyp.Status, "DB version is " + dbVersion.ToString());
                return dbVersion;
            }                   
        }


        public void UpgradeDb()
        {
            switch (GetDbVersion())
            {
                case 1:
                    UpdgradeToDbVersion_2();
                    break;
                case 2:
                    Logger.Log(LogEintragTyp.Hinweis, "DB is up to date.");
                    break;
            }
        }

        private void UpdgradeToDbVersion_2()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SET TERM ^ ;");
            sql.AppendLine("CREATE TRIGGER TASKPAUSE_BI FOR TASKPAUSE ACTIVE");
            sql.AppendLine("BEFORE INSERT POSITION 0");
            sql.AppendLine("AS");
            sql.AppendLine("DECLARE VARIABLE tmp DECIMAL(18,0);");
            sql.AppendLine("BEGIN");
            sql.AppendLine("IF (NEW.ID IS NULL) THEN");
            sql.AppendLine("NEW.ID = GEN_ID(GEN_TASKPAUSE_ID, 1);");
            sql.AppendLine("ELSE");
            sql.AppendLine("BEGIN");
            sql.AppendLine("tmp = GEN_ID(GEN_TASKPAUSE_ID, 0);");
            sql.AppendLine("if (tmp < new.ID) then");
            sql.AppendLine("tmp = GEN_ID(GEN_TASKPAUSE_ID, new.ID-tmp);");
            sql.AppendLine("END");
            sql.AppendLine("END^");
            sql.AppendLine("SET TERM ; ^");

            sql.AppendLine("ALTER TABLE TASKPAUSE ADD CONSTRAINT PK_TASKPAUSE_1 PRIMARY KEY (ID);");
            sql.AppendLine("GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE");
            sql.AppendLine("ON TASKPAUSE TO  SYSDBA WITH GRANT OPTION;");

            context.Database.ExecuteSqlCommand(sql.ToString());
            context.SaveChanges();
            context.DbVersion.Add(new DbVersionModel() { Version = 1, Descripiton = "Inital Db Version with version table" });
            context.SaveChanges();
            UpgradeDb();
        }

   }
}
