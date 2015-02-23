namespace NHibernate.Sessions.Operations
{
	public abstract class CachedDatabaseQuery<T> : AbstractCachedDatabaseQuery<T>, ICachedDatabaseQuery<T>
	{
		public virtual T Execute(ISessionManager sessionManager, IDatabaseQueryCache databaseQueryCache = null)
		{
			return GetDatabaseResult(sessionManager, databaseQueryCache);
		}
	}
}