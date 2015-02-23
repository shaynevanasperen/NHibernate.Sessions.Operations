namespace NHibernate.Sessions.Operations
{
	public abstract class DatabaseQuery<T> : DatabaseOperation, IDatabaseQuery<T>
	{
		public abstract T Execute(ISessionManager sessionManager);
	}
}
