namespace NHibernate.Sessions.Operations
{
	public interface ICachedDatabaseQuery<out T>
	{
		T Execute(ISessionManager sessionManager, IDatabaseQueryCache databaseQueryCache = null);
	}
}