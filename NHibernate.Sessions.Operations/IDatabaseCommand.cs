namespace NHibernate.Sessions.Operations
{
	public interface IDatabaseCommand
	{
		void Execute(ISessionManager sessionManager);
	}

	public interface IDatabaseCommand<out T>
	{
		T Execute(ISessionManager sessionManager);
	}
}