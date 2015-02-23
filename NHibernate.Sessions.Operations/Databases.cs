namespace NHibernate.Sessions.Operations
{
	public class Databases : IDatabases
	{
		readonly IDatabaseQueryCache _databaseQueryCache;

		public Databases(ISessionManager sessionManager, IDatabaseQueryCache databaseQueryCache = null)
		{
			SessionManager = sessionManager;
			_databaseQueryCache = databaseQueryCache;
		}

		public ISessionManager SessionManager { get; private set; }

		public T Query<T>(IDatabaseQuery<T> query)
		{
			return query.Execute(SessionManager);
		}

		public T Query<T>(ICachedDatabaseQuery<T> query)
		{
			return query.Execute(SessionManager, _databaseQueryCache);
		}

		public void Command(IDatabaseCommand command)
		{
			command.Execute(SessionManager);
		}

		public T Command<T>(IDatabaseCommand<T> command)
		{
			return command.Execute(SessionManager);
		}
	}
}
