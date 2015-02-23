namespace NHibernate.Sessions.Operations
{
	public abstract class DatabaseCommand : DatabaseOperation, IDatabaseCommand
	{
		public abstract void Execute(ISessionManager sessionManager);
	}

	public abstract class DatabaseCommand<T> : DatabaseOperation, IDatabaseCommand<T>
	{
		public abstract T Execute(ISessionManager sessionManager);
	}
}
