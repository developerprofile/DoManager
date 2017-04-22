using System.Data.Entity;

namespace ch.jaxx.TaskManager.DataAccess
{
	class FirebirdContextConfiguration : DbConfiguration
	{
		public FirebirdContextConfiguration()
		{
			SetDatabaseInitializer<FirebirdContext>(new CreateDatabaseIfNotExists<FirebirdContext>());
		}
	}
}
