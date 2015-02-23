namespace NHibernate.Sessions.Operations
{
	public interface IDatabaseQuery<out T>
	{
		T Execute(ISessionManager sessionManager);
	}
}