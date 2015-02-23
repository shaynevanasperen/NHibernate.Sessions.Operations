namespace NHibernate.Sessions.Operations
{
	public abstract class TransformedCachedDatabaseQuery<TDatabaseResult, TTransformedResult> : AbstractCachedDatabaseQuery<TDatabaseResult>, ICachedDatabaseQuery<TTransformedResult>
	{
		protected abstract TTransformedResult TransformDatabaseResult(TDatabaseResult databaseResult);

		public TTransformedResult Execute(ISessionManager sessionManager, IDatabaseQueryCache databaseQueryCache = null)
		{
			return TransformDatabaseResult(GetDatabaseResult(sessionManager, databaseQueryCache));
		}
	}
}