namespace NHibernate.Sessions.Operations
{
	public interface IDatabases
	{
		ISessionManager SessionManager { get; }

		T Query<T>(IDatabaseQuery<T> query);
		T Query<T>(ICachedDatabaseQuery<T> query);

		void Command(IDatabaseCommand command);
		T Command<T>(IDatabaseCommand<T> command);
	}
}